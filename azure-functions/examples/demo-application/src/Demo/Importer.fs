module NemeStats.Import.Importer

open System.Threading

open FSharp.Control.Rop.TaskResult

open FSharp.Control.Rop
open NemeStats.Import.BoardGameGeek
open NemeStats.Import.BoardGameGeek.Functions
open NemeStats.Import.NemeStats



//TODO 
/// Return BGG players that are not created in NemeStats
let getMissingPlayers cancellationToken bggUsername (gamingGroup: GamingGroup) dateFrom dateTo =
    taskResult {
        let! bggPlays = BoardGameGeek.Api.getPlays cancellationToken bggUsername dateFrom (dateTo |> Option.defaultValue Date.Today)
        let! nemePlayers = NemeStats.Api.Players.getPlayersInGamingGroup cancellationToken gamingGroup
        
        let nemePlayerNameSet = nemePlayers |> List.map (fun x -> Some x.Name) |> Set.ofList
        
        return
            bggPlays
            |> getUniquePlayers
            |> List.where (fun x ->
                nemePlayerNameSet |> Set.contains x.Name |> not)
    }
    
    
    
    
//TODO handle UNKNOWN PLAYERS - some higher level import logic, necessary for importing games


let createPlayers getCanonicalName cancellationToken authenticationToken gamingGroup (players: PlayerIdentification list) =
    taskResult {
        return!
            players
            |> List.choose (fun x -> x.Name |> Option.map getCanonicalName)
            |> List.distinct 
            |> List.map (fun x -> Api.Players.createPlayer cancellationToken authenticationToken gamingGroup x)
            |> TaskResult.bisequence
    }    
    

let private asyncTimeoutMs = 30000
let private maxConcurrentCalls = 5

let private runSync cancellationToken x =
    Async.RunSynchronously(x, timeout = asyncTimeoutMs, cancellationToken = cancellationToken )

/// alternative implementation that limits number of concurrent calls from this function at the cost of blocking thread
/// TODO rewrite to use Semaphore or TPL DataFlow
let createPlayersThrottled getCanonicalName cancellationToken authenticationToken gamingGroup (players: PlayerIdentification list) =
        players
        |> List.choose (fun x -> x.Name |> Option.map getCanonicalName)
        |> List.distinct
        |> List.chunkBySize maxConcurrentCalls
        |> List.map (fun chunk ->
                chunk
                |> List.map (fun x -> Api.Players.createPlayer cancellationToken authenticationToken gamingGroup x)
                |> List.map (fun t -> t |> Async.AwaitTask |> runSync cancellationToken )
            )
        |> List.concat
        |> bisequence
            
      