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

open Newtonsoft.Json

open NemeStats.Import
open NemeStats.Import.BoardGameGeek
open NemeStats.Import.Importer

open Demo.FunctionApp

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
    
        
    [<Literal>]
    let private ImportQueue = "importQueue"
    
    [<Literal>]
    let private ParseGamesQueue = "parseGames"
    
    [<Literal>]
    
    let private AzureStorageConnection = "AzureWebJobsStorage"
        
        
    [<FunctionName("HTTP-RunImport")>]        
    let runImportHTTP
        ([<HttpTrigger(AuthorizationLevel.Function, "post", Route = "import/run")>]req: HttpRequest)
        ([<Queue(ImportQueue, Connection = AzureStorageConnection)>] [<Out>] queueMessage: ImportParameters outref)
        (context: Microsoft.Azure.WebJobs.ExecutionContext)
        (log: ILogger) =
        
        let q x = queryParam x req 
      
        match q "bggusername", q "datefrom", q "dateto" with
          | (Some bgguser, Some dateFrom, dateTo) ->

              queueMessage <-
                  {
                    BggUsername = bgguser
                    //NOTE: this is not how this should work, as for invalid strings we will get an unreadable exception.
                    DateFrom = Date.fromString dateFrom |> Option.get  
                    DateTo = dateTo |> Option.bind Date.fromString |> Option.defaultValue Date.Today
                  }

              log.LogInformation("Enqueued import of data for user {0}", bgguser)              
              OkObjectResult("Enqueued") :> IActionResult
          | (bgguser, dateFrom, dateTo) ->
              let importParametersResult = 
                  getConfigurationRoot context
                  |> Configuration.DefaultImportParameters.get
                  |> Result.map (fun cfg ->
                      {
                        BggUsername = bgguser |> Option.defaultValue cfg.BggUser
                        DateFrom = dateFrom |> Option.bind Date.fromString  |> Option.defaultValue cfg.DateFrom
                        DateTo = dateTo |> Option.bind Date.fromString |> Option.defaultValue Date.Today
                      })
              
              match importParametersResult with
              | Ok m ->
                  queueMessage <- m
                  log.LogInformation("Enqueued import of data for user {0}. Some data was read from configuration", m.BggUsername)
                  OkObjectResult("Enqueued") :> IActionResult
              | Error _ -> BadRequestObjectResult("Please fill in all required parameters. ") :> IActionResult 
                  
              
       
       
    //binding expression patterns see https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-expressions-patterns              
    [<FunctionName("QUEUE-DownloadData")>]              
    let downloadDataQueue
        ([<QueueTrigger(ImportQueue, Connection = AzureStorageConnection)>]importPars: ImportParameters)
        ([<Blob("bggdata/{rand-guid}.xml", FileAccess.Write, Connection = AzureStorageConnection)>]bggData: Stream)
        (log: ILogger) =
            task {
                log.LogInformation <| sprintf "Triggered Download data: %A" importPars
                
                //NOTE: we are just downloading and processing the first page of data.
                //could also access BlobDirectory directly https://social.msdn.microsoft.com/Forums/azure/en-US/f0b71c54-c92c-4180-9896-70a4d1209ca2/naming-blob-output?forum=AzureFunctions
                //or we could use IBinder to create several blobs for given username
                let getPage pIndex =
                    BoardGameGeek.Api.downloadPlaysPage importPars.BggUsername importPars.DateFrom importPars.DateTo pIndex

                    
                match! getPage 0 with
                | Ok p ->
                    use writer = new StreamWriter(bggData)
                    do! writer.WriteAsync p
                | Error e ->
                    failwithf "Error downloading data: %A" e
                    
                log.LogInformation("Downloaded plays for player {0}", importPars.BggUsername)                    
                //return sprintf "bggdata/%s.xml" importPars.BggUsername               
            }
            
       
       
    [<CLIMutable>]       
    type PlayersTable = {
        PartitionKey: string
        RowKey: string
        Players: string list
    }
    
    
    let private readAsString (bggData: Stream) =
        use reader = new StreamReader(bggData)
        reader.ReadToEnd () 
        
           
    [<FunctionName("BLOB-ParsePlayers")>]       
    let parsePlayers
        ([<BlobTrigger("bggdata/{name}", Connection = AzureStorageConnection )>]bggData: Stream)
        (name: string) //name of blob
        ([<Queue(ParseGamesQueue, Connection = AzureStorageConnection)>] [<Out>] blobToParseGames: string outref)
        // https://stackoverflow.com/questions/8467227/apply-attribute-to-return-value-in-f
        (log: ILogger)
        // return attribute - store data to specified table. We have to use unique RowKey because this is always inserted
        : [<return: Table("testTable")>] PlayersTable =
            
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
                log.LogInformation <| sprintf "Parse games from blob: %s" blobName
                
                let blobAttribute = BlobAttribute(sprintf "bggdata/%s" blobName, FileAccess.Read)
                use! bggData = blobBinder.BindAsync<Stream>(blobAttribute)
                
                let xmlString = readAsString bggData
                let games =
                    xmlString
                    |> BoardGameGeek.Api.parsePlays
                    |> List.map (fun x -> x.GameName)
                    |> List.distinct
                    
                log.LogInformation <| sprintf "Found following games played: %A" games                    
            }
            
            
            
            
    type PlayersTableEntity() =
        inherit TableEntity()
        // https://stackoverflow.com/questions/45517481/create-a-tableentity-with-array-or-list-property
        member val Players : string list = [] with get, set
        
        member this.PlayersRaw
            with get () =
                this.Players |> JsonConvert.SerializeObject
            and  set (value) =
                match value with
                | null -> ()
                | _ -> this.Players <- JsonConvert.DeserializeObject<string list>(value) 

        
        
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