module Demo.FunctionApp.Configuration

open Microsoft.Extensions.Configuration

open System
open System.Text.RegularExpressions

open FSharp.Control.Rop


type ConfigurationError = 
    | MissingConfigurationValue of key: string
    with 
        member x.Explain () =
            match x with
            | MissingConfigurationValue k -> sprintf "Missing configuration value '%s'" k



module private Core = 
    let valueOrDefault defaultValue value =
        match value with
        | null -> defaultValue
        | _ -> value
        
        
    let valueOrNone  value =
        match value with
        | null -> None
        | _ -> Some value  

    let parseOrDefault parseFn defaultValue value =
        match parseFn value with
        | (true, v) -> v
        | (false, _) -> defaultValue

    let trimSecret (s: string) =
        match s with
        | null -> ""
        | x when x.Length > 3 -> sprintf "%s*******" (s.Substring(0, 3))
        | _ -> s

    let private passwordRegex = Regex("Password=.[^;]*", RegexOptions.Compiled)
    let removePasswordFromConnectionString s = 
        match s with
        | null -> ""
        | _ -> passwordRegex.Replace(s, fun _ -> "Password=*******")

    let private sharedAccessKeyRegex = Regex("SharedAccessKey=.[^;]*", RegexOptions.Compiled)
    let removeSharedAccessKeyFromConnectionString s = 
        match s with
        | null -> ""
        | _ -> sharedAccessKeyRegex.Replace(s, fun _ -> "SharedAccessKey=*******")    


    let validateFilledKey name (conf: IConfiguration) =
        match conf.Item name with
        | null ->  MissingConfigurationValue name |> List.singleton |> Error
        | _ -> Ok conf    

    /// Validates parameter against a list of validation functions. Returns parameter or validation errors
    let validate (fList: ('a -> Result<'a, 'e list>) list) (x: 'a): Result<'a, 'e list> = 
        match fList with
        | [] -> Ok x
        | _ -> List.fold
                (fun xR f ->
                    match (xR, f x) with
                    | Ok x, Ok _ -> Ok x
                    | Error x, Ok _ -> Error x
                    | Ok _, Error y -> Error y
                    | Error x, Error y -> List.concat [x ; y ] |> Error
                )
                (Ok x)
                fList   

    /// apply implementation that collects all errors
    let apply fR xR =
        match fR, xR with
        | Ok f, Ok x -> Ok (f x)
        | Ok _, Error e -> Error e
        | Error e, Ok _ -> Error e
        | Error x, Error y -> List.concat [x ; y ] |> Error

    let (<*>) = apply


module DefaultImportParameters =


    open Core
    
    open NemeStats.Import
    
    type DefaultParamsConfig =
        {
            DateFrom: Date.Date
            BggUser: string
        }
        
    
    let get (conf: IConfiguration) : Result<DefaultParamsConfig, ConfigurationError list> =
        conf 
        |> validateFilledKey "defaultParams:startDate" 
        >>= validateFilledKey "defaultParams:bggUser" 
        <!> (fun x -> {
            DateFrom = x.["defaultParams:startDate"] |> Date.fromString |> Option.defaultValue (new DateTime(2018,1,1) |> Date.fromDateTime)
            BggUser = x.["defaultParams:bggUser"]
        })



