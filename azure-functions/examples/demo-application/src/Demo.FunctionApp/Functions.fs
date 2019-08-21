module NemeStats.Import.FunctionApp.Functions

open System
open System.IO

open FSharp.Control.Tasks.V2

open Microsoft.AspNetCore.Mvc
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging

open Newtonsoft.Json

open System.Threading

open FSharp.Control.Rop
open Microsoft.Azure.WebJobs
open Microsoft.WindowsAzure.Storage.Table
open System.Runtime.InteropServices
open System.Text.Unicode
open NemeStats.Import
open NemeStats.Import.BoardGameGeek
open NemeStats.Import.Importer
open NemeStats.Import.NemeStats

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
    
    //TODO remove later
    [<FunctionName("HTTP-ImportPlayers")>]
    let importPlayers
        ([<HttpTrigger(AuthorizationLevel.Function, "post", Route = "import/users")>]req: HttpRequest)
        ([<Blob("fff/stub.jpg", FileAccess.Read)>] myBlob: byte[])
        (log: ILogger) =
        task {
          log.LogInformation(sprintf "blob size = %i" (myBlob |> Array.length))
          log.LogInformation("F# HTTP trigger function processed a request.")
          let q x = queryParam x req 
          
          match q "bggusername", q "gamingGroupId", q "datefrom", q "dateto" with
          | (Some bgguser, Some groupId, Some dateFrom, dateTo) ->
              let gamingGroup = createGamingGroup groupId
              //TODO error handling, parameter validation (applicative, see Config handling)
              let dateFromD = Date.fromString dateFrom |> Option.get
              let dateToD = dateTo |> Option.bind Date.fromString
              
              let! missing = Importer.getMissingPlayers CancellationToken.None bgguser gamingGroup dateFromD dateToD
              
              let result =
                  missing
                  |> Result.map (fun x -> x |> List.choose (fun y -> y.Name))
              
              //TODO return JSON, from record using standard serialization
              return OkObjectResult(sprintf "Missing players result: %A" result) :> IActionResult
          | _ ->
              return BadRequestObjectResult("TBD, please") :> IActionResult              
          
          
        }
        
    [<Literal>]
    let private ImportQueue = "importQueue"
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
        (log: ILogger) =
        
        let q x = queryParam x req 
      
        match q "bggusername", q "gamingGroupId", q "datefrom", q "dateto" with
          | (Some bgguser, Some groupId, Some dateFrom, dateTo) ->

              //TODO error handling, parameter validation (applicative, see Config handling)
              queueMessage <-
                  {
                    BggUsername = bgguser
                    DateFrom = Date.fromString dateFrom |> Option.get
                    DateTo = dateTo |> Option.bind Date.fromString |> Option.defaultValue Date.Today
                    GamingGroup = createGamingGroup groupId
                  }

              log.LogInformation("Enqueued import of data for user {0}", bgguser)              
              OkObjectResult("Enqueued") :> IActionResult
          | _ ->
              BadRequestObjectResult("Please fill in all required parameters.") :> IActionResult
              
       
       
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