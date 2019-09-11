module NemeStats.Import.FunctionApp.Functions

open System
open System.IO
open System.Runtime.InteropServices

open FSharp.Control.Tasks.V2

open Microsoft.AspNetCore.Mvc
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Configuration

open Microsoft.WindowsAzure.Storage.Table

open NemeStats.Import
open NemeStats.Import.BoardGameGeek
open NemeStats.Import.Importer

open Demo.FunctionApp
open Demo.FunctionApp.Storage

module Functions =
    
    let queryParam key (req: HttpRequest) =
        if req.Query.ContainsKey(key) then
                    Some(req.Query.[key].[0])
                else
                    None
    
        
    let private getConfigurationRoot (context: Microsoft.Azure.WebJobs.ExecutionContext) =
        (ConfigurationBuilder())
            .SetBasePath(context.FunctionAppDirectory)
            .AddJsonFile("local.settings.json", true)
            .AddEnvironmentVariables()
            .Build();        
    
    
    // non-Result method for demo, only gets first page of results
    let private downloadPlays (pars: ImportParameters) =
        task {
            let! data = BoardGameGeek.Api.downloadPlaysPage pars.BggUsername pars.DateFrom pars.DateTo 0
            match data with
            | Ok x ->
                return x
            | Error e ->
                failwithf "%A" e
                return "" //NOTE: this row won't execute but we have to satisfy the compiler
        }
        
        
    [<Literal>]
    let private ImportQueue = "importQueue"
    
    [<Literal>]
    let private ParseGamesQueue = "parseGames"
    
    [<Literal>]
    let private AzureStorageConnection = "AzureWebJobsStorage"
        
        
        
        
        
        
    [<FunctionName("HTTP-RunImport")>]        
    let runImportHTTP
        ([<HttpTrigger(AuthorizationLevel.System, "post", Route = "import/run")>]req: HttpRequest)
        ([<Queue(ImportQueue, Connection = AzureStorageConnection)>] [<Out>] queueMessage: ImportParameters outref)
        (context: Microsoft.Azure.WebJobs.ExecutionContext)
        (log: ILogger) =
        
        let q x = queryParam x req
        
        let cfg = getConfigurationRoot context |> Configuration.DefaultImportParameters.parse
        
        let bgguser = q "bggusername" |> Option.defaultValue cfg.BggUser
        let datefrom = q "datefrom" |> Option.bind Date.fromString |> Option.defaultValue cfg.DateFrom
        let dateto = q "dateto" |> Option.bind Date.fromString |> Option.defaultValue Date.Today
        
        log.LogInformation("Enqueueing import of data for user {0}", bgguser)
        queueMessage <- ImportParameters.Create bgguser datefrom dateto
            
        OkObjectResult("OK") :> IActionResult

      
      
      
      
      
    //binding expression patterns see https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-expressions-patterns              
    [<FunctionName("QUEUE-DownloadData")>]              
    let downloadDataQueue
        ([<QueueTrigger(ImportQueue, Connection = AzureStorageConnection)>]importPars: ImportParameters)
        ([<Blob("bggdata/{rand-guid}.xml", FileAccess.Write, Connection = AzureStorageConnection)>]bggData: Stream)
        (log: ILogger) =
            task {
                log.LogInformation ("Triggered Download data: {0}", importPars)
                
                let! data = downloadPlays importPars
                use writer = new StreamWriter(bggData)
                do! writer.WriteAsync data
                    
                log.LogInformation("Downloaded plays for player {0}", importPars.BggUsername)                    
            }    

    
    
    
          
    [<FunctionName("BLOB-ParsePlayers")>]       
    let parsePlayers
        ([<BlobTrigger("bggdata/{name}", Connection = AzureStorageConnection )>]bggData: Stream)
        (name: string) //name of blob
        ([<Queue(ParseGamesQueue, Connection = AzureStorageConnection)>] [<Out>] blobToParseGames: string outref)
        // https://stackoverflow.com/questions/8467227/apply-attribute-to-return-value-in-f
        (log: ILogger)
        // return attribute - store data to specified table. We have to use unique RowKey because this is always inserted
        : [<return: Table("testTable")>] PlayersTable =
            
            log.LogInformation ("Parsing data from blob {0}", name)
            //enqueue blob name to parse games    
            blobToParseGames <- name
                
            let xmlString = readAsString bggData
            let players =
                xmlString
                |> BoardGameGeek.Api.parsePlays
                |> Functions.getUniquePlayers
            
            {
                PartitionKey = "bgg"
                RowKey = xmlString |> BoardGameGeek.Api.parseUsername |> (sprintf "%s%s" (Guid.NewGuid().ToString()) )
                Players = players |> List.choose (fun x -> x.Name)
            }
            
            
            
            
            
            
            
            
    // https://simonholman.blog/azure-functions-with-imperative-bindings/            
    [<FunctionName("QUEUE-ParseGames")>]              
    let parseGamesQueue
        ([<QueueTrigger(ParseGamesQueue, Connection = AzureStorageConnection)>]blobName: string)
        (blobBinder: IBinder)
        (log: ILogger) =
            task {
                log.LogInformation("Parse games from blob: {0}", blobName)
                
                let blobAttribute = BlobAttribute(sprintf "bggdata/%s" blobName, FileAccess.Read)
                use! bggData = blobBinder.BindAsync<Stream>(blobAttribute)
                
                let xmlString = readAsString bggData
                let games =
                    xmlString
                    |> BoardGameGeek.Api.parsePlays
                    |> List.map (fun x -> x.GameName)
                    |> List.distinct
                    
                log.LogInformation("Found following games played: {0}", games)                    
            }
            

    /// Alternative example for binding to Table Storage. This Function stores data using CloudTable which allows us to update existing values        
    //needs Microsoft.WindowsAzure.Storage.dll 9.3.1, workaround here https://github.com/Azure/azure-functions-host/issues/3784.             
    [<FunctionName("BLOB-ParsePlayersEntity")>]       
    let parsePlayersRepeat
        ([<BlobTrigger("bggdata/{name}", Connection = AzureStorageConnection )>]bggData: Stream)
        ([<Table("bggPlayers")>] table: CloudTable)
        (log: ILogger) =
            task {
                use reader = new StreamReader(bggData)
                let xmlString = reader.ReadToEnd () 
                let players =
                    xmlString
                    |> BoardGameGeek.Api.parsePlays
                    |> Functions.getUniquePlayers
                
                let partitionKey = "bgg"
                let rowKey = xmlString |> BoardGameGeek.Api.parseUsername
                
                let playersTable = PlayersTableEntity()
                playersTable.RowKey <- rowKey
                playersTable.PartitionKey <- partitionKey
                playersTable.Players <- players |> List.choose (fun x -> x.Name)
                
                //https://microsoft.github.io/AzureTipsAndTricks/blog/tip85.html
                let retrieve = TableOperation.Retrieve<PlayersTableEntity>(partitionKey, rowKey);
                let! retrieveResult = table.ExecuteAsync(retrieve)
                let operation =
                    match retrieveResult with
                    | null
                    | _ when retrieveResult.HttpStatusCode <> 200 ->
                        TableOperation.Insert(playersTable)
                    | original ->
                        let tbl = original.Result :?> PlayersTableEntity
                        tbl.Players <- (tbl.Players |> List.append playersTable.Players |> List.distinct)
                        tbl.ETag <- original.Etag
                        
                        TableOperation.Replace(tbl)
                let! opResult = table.ExecuteAsync(operation)
                
                log.LogInformation("Data processed, result = {0}", opResult.HttpStatusCode)
                ()
            }