module NemeStats.Import.BoardGameGeek.Api

open System
open System.Threading
open System.Threading.Tasks


open FSharp.Control.Tasks.V2
open FSharp.Control.Rop.TaskResult
open FSharp.Data

open FSharp.Control.Rop
open NemeStats.Import
open NemeStats.Import.Date


type private Plays =
    XmlProvider<"""BoardGameGeek/SampleData/plays.xml"""
    ,EmbeddedResource = """NemeStats.Import, NemeStats.Import.BoardGameGeek.SampleData.plays.xml""">

let private baseUri = "https://www.boardgamegeek.com/xmlapi2"

let private getPlaysUri username (datefrom: Date) (dateTo: Date) page =
    sprintf "%s/plays?username=%s&page=1&mindate=%s&maxdate=%s&page=%i"
        //NOTE: API pages are indexed from 1 but we want indexing from zero
        baseUri username (datefrom |> Date.value) (dateTo |> Date.value) (page + 1)
    |> Uri        


module private Mapping = 

    let private zeroToNone = function
        | 0 -> None
        | n -> Some n

    let private emptyStringToNone = function
        | null | "" -> None
        | x -> Some x
    
    let xmlPlayerToPlayer (player: Plays.Player) =
        {
            PlayerIdentification =
                {
                    UserId = player.Userid |> zeroToNone
                    Username = player.Username |> Option.bind emptyStringToNone
                    Name = player.Name |> Option.bind emptyStringToNone
                }
            Score = player.Score
            Win = player.Win
        }

    let xmlPlayToPlay (play: Plays.Play) : Play =
        {
            Id = play.Id
            Date = play.Date |> Date.fromDateTime
            Quantity = play.Quantity
            Incomplete = play.Incomplete
            DontTrackWinStats = play.Nowinstats = 1
            Location = play.Location |> Option.defaultValue ""
            GameId = play.Item.Objectid
            GameName = play.Item.Name
            Comments = play.Comments
            Players = play.Players |> Array.map xmlPlayerToPlayer |> List.ofArray
        }

    let xmlPlaysToPlays (plays: Plays.Plays) =
        plays.Plays
        |> Array.map xmlPlayToPlay
        |> List.ofArray

let getPageCount (p: Plays.Plays) =
    p.Total / 100 + 1



let getPlays cancellationToken username (dateFrom: Date) (dateTo: Date) =

    taskResult {
        let getPage pIndex =
            getPlaysUri username dateFrom dateTo pIndex
            |> Http.createRequest System.Net.Http.HttpMethod.Get
            |> Http.getResponse cancellationToken
            |> TaskResult.map Plays.Parse

        let headPage = getPage 0
        
        let! pageCount =
            headPage
            |> TaskResult.map getPageCount
        
        let tailPages =
            [ 1 .. pageCount ]
            |> List.map getPage
        
        let! xmlPlays =
            headPage :: tailPages
            |> TaskResult.sequence
            
        return
            xmlPlays
            |> List.collect Mapping.xmlPlaysToPlays
    }