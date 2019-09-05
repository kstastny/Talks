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
open NemeStats.Import.NemeStats

open Demo.FunctionApp

module Functions =
    
    let queryParam key (req: HttpRequest) =
        if req.Query.ContainsKey(key) then
                    Some(req.Query.[key].[0])
                else
                    None
    
    let createGamingGroup queryParam : GamingGroup =
        {
            //TODO error handling
            Id = queryParam |> Int32.Parse
        }
        
    let private getConfigurationRoot (context: Microsoft.Azure.WebJobs.ExecutionContext) =
        (ConfigurationBuilder())
            .SetBasePath(context.FunctionAppDirectory)
            .AddJsonFile("local.settings.json", true)
            .AddEnvironmentVariables()
            .Build();        
    
        
    [<Literal>]
    let private ImportQueue = "importQueue"
    
    //TODO all connections are the same, it is just StorageConnection
    [<Literal>]        
    let private QueuesConnection = "AzureWebJobsStorage"
    [<Literal>]        
    let private BlobConnection = "AzureWebJobsStorage"
    [<Literal>]        
    let private TablesConnection = "AzureWebJobsStorage"
        
    [<FunctionName("HTTP-RunImport")>]        
    let runImportHTTP
        ([<HttpTrigger(AuthorizationLevel.Function, "post", Route = "import/run")>]req: HttpRequest)
        ([<Queue(ImportQueue, Connection = QueuesConnection)>] [<Out>] queueMessage: ImportParameters outref)
        (context: Microsoft.Azure.WebJobs.ExecutionContext)
        (log: ILogger) =
        
        let q x = queryParam x req 
      
        match q "bggusername", q "datefrom", q "dateto" with
          | (Some bgguser, Some dateFrom, dateTo) ->

              queueMessage <-
                  {
                    BggUsername = bgguser
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
        ([<QueueTrigger(ImportQueue, Connection = QueuesConnection)>]importPars: ImportParameters)
        ([<Blob("bggdata/{rand-guid}.xml", FileAccess.Write, Connection = BlobConnection)>]bggData: Stream)
        (log: ILogger) =
            task {
                let msg = sprintf "F# Queue trigger function processed: %A" importPars
                log.LogInformation msg
                
                //TODO more pages in different function - will need different logic and more blobs (imperative bindings, different example, see https://simonholman.blog/azure-functions-with-imperative-bindings/)
                //could also access BlobDirectory directly https://social.msdn.microsoft.com/Forums/azure/en-US/f0b71c54-c92c-4180-9896-70a4d1209ca2/naming-blob-output?forum=AzureFunctions
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
           
    [<FunctionName("BLOB-ParsePlayers")>]       
    let parsePlayers
        //TODO remove blob after processing?
        ([<BlobTrigger("bggdata/{name}", Connection = BlobConnection )>]bggData: Stream)
        // https://stackoverflow.com/questions/8467227/apply-attribute-to-return-value-in-f
        (log: ILogger) 
        : [<return: Table("testTable")>] PlayersTable =
            use reader = new StreamReader(bggData)
            let xmlString = reader.ReadToEnd () 
            let players =
                xmlString
                |> BoardGameGeek.Api.parsePlays
                |> Functions.getUniquePlayers
            
            {
                PartitionKey = "bgg"
                //RowKey = xmlString |> BoardGameGeek.Api.parseUsername
                RowKey = xmlString |> BoardGameGeek.Api.parseUsername |> (sprintf "%s%s" (Guid.NewGuid().ToString()) )
                Players = players |> List.choose (fun x -> x.Name)
            }
            
            
            
    type PlayersTable2() =
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

        
        
    //needs Microsoft.WindowsAzure.Storage.dll 9.3.1, workaround here https://github.com/Azure/azure-functions-host/issues/3784.             
    [<FunctionName("BLOB-ParsePlayers2")>]       
    let parsePlayersRepeat
        //TODO another output binding - queue that will continue - e.g. parseGames, just to show to outputs 
        ([<BlobTrigger("bggdata/{name}", Connection = BlobConnection )>]bggData: Stream)
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
                
                let playersTable = PlayersTable2()
                playersTable.RowKey <- rowKey
                playersTable.PartitionKey <- partitionKey
                playersTable.Players <- players |> List.choose (fun x -> x.Name)
                
                //https://microsoft.github.io/AzureTipsAndTricks/blog/tip85.html
                let retrieve = TableOperation.Retrieve<PlayersTable2>(partitionKey, rowKey);
                let! retrieveResult = table.ExecuteAsync(retrieve)
                let operation =
                    match retrieveResult with
                    | null
                    | _ when retrieveResult.HttpStatusCode <> 200 ->
                        TableOperation.Insert(playersTable)
                    | original ->
                        let tbl = original.Result :?> PlayersTable2
                        tbl.Players <- (tbl.Players |> List.append playersTable.Players |> List.distinct)
                        tbl.ETag <- original.Etag
                        
                        //printfn "PL = %A" tbl.Players
                        
                        TableOperation.Replace(tbl)
                let! opResult = table.ExecuteAsync(operation)
                
                log.LogInformation("Data processed, result = {0}", opResult.HttpStatusCode)
                ()
            }