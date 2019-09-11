/// Mapping to Azure Storage and helper functions
module Demo.FunctionApp.Storage

open System
open System.IO

open Microsoft.WindowsAzure.Storage.Table

open Newtonsoft.Json


/// list of players in TableStorage
[<CLIMutable>]       
type PlayersTable = {
    PartitionKey: string
    RowKey: string
    Players: string list
}


// /// list of players in TableStorage - alternative mapping as TableEntity
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

let readAsString (data: Stream) =
    use reader = new StreamReader(data)
    reader.ReadToEnd () 