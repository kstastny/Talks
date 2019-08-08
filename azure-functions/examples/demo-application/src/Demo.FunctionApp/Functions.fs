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

open NemeStats.Import
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