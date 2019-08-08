module NemeStats.Import.NemeStats.Api

open System
open System.Threading
open System.Threading.Tasks


open FSharp.Data
open FSharp.Control.Rop.TaskResult


open NemeStats.Import
open NemeStats.Import
open NemeStats.Import.Http

let baseUri = "https://nemestats.com/api/v2"

type ResponseFailure =
    {
        Message: string
    }

module Authentication =
    
    let private authenticationEndpoint = sprintf "%s/UserSessions/" baseUri |> Uri
    
    type AuthenticationRequest =
        {
            Username: string
            Password: string
            UniqueDeviceId: string
        }
    
    type AuthenticationResponse =
        {
            AuthenticationToken: string
            AuthenticationTokenExpirationDateTime: DateTime
        }
    
    let private getAuthenticationBody username password deviceId =
        { Username = username; Password = password; UniqueDeviceId = deviceId }
        |> Serialization.serialize
    
    
    let authenticate username password uniqueDeviceId =
        
        taskResult {
            let requestJson = getAuthenticationBody username password uniqueDeviceId
            
            let! response =
                Http.createRequest System.Net.Http.HttpMethod.Post authenticationEndpoint
                |> Http.setContent requestJson "application/json"
                |> Http.getResponse CancellationToken.None
            
            let token = Serialization.deserialize<AuthenticationResponse> response
            return
                {
                    Token = token.AuthenticationToken
                    Expiration = token.AuthenticationTokenExpirationDateTime
                }
        }
        
        

module Players = 

    type private PlayersInGamingGroupJson =
        JsonProvider<"""NemeStats/SampleData/GetPlayersInGamingGroup.json"""
        ,EmbeddedResource = """NemeStats.Import, NemeStats.Import.NemeStats.SampleData.GetPlayersInGamingGroup.json""">
        
    let private playersEndpoint = sprintf "%s/Players" baseUri        

    let private getPlayersInGamingGroupUri (gamingGroup: GamingGroup) =
        sprintf "%s?gamingGroupId=%i" playersEndpoint gamingGroup.Id
        |> Uri


    let private jsonToPlayers (root: PlayersInGamingGroupJson.Root) =
        root.Players
        |> Array.map (fun x -> {
            Id = x.PlayerId
            Name = x.PlayerName
        })
        
        
    let getPlayersInGamingGroup (cancellationToken: CancellationToken) (gamingGroup: GamingGroup) : Task<Result<Player list, HttpError>> =
        
        taskResult {
            let! response =
                getPlayersInGamingGroupUri gamingGroup
                |> Http.createRequest System.Net.Http.HttpMethod.Get
                |> Http.getResponse cancellationToken
            
            return
                PlayersInGamingGroupJson.Parse response
                |> jsonToPlayers
                |> List.ofArray
        }
        
            
    type CreatePlayerRequest =
        {
            PlayerName: string
        }
        
    type CreatePlayerResponse =
        {
            PlayerId: int
            GamingGroupId: int
            NemeStatsUrl: string
        }
        
    let createPlayer (cancellationToken: CancellationToken) (authenticationToken: AuthenticationToken) (gamingGroup: GamingGroup) name =
        
        taskResult {
            let requestJson = {PlayerName = name } |> Serialization.serialize
            
            let! response =
                Http.createRequest System.Net.Http.HttpMethod.Post (getPlayersInGamingGroupUri gamingGroup)
                |> Http.setContent requestJson "application/json"
                |> Http.addHeader "X-Auth-Token" authenticationToken.Token
                |> Http.getResponse cancellationToken
            
            return Serialization.deserialize<CreatePlayerResponse> response
        }
