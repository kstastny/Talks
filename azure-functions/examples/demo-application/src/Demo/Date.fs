// https://gist.github.com/swlaschin/54cfff886669ccab895a + https://fsharpforfunandprofit.com/posts/designing-with-types-more-semantic-types/

module NemeStats.Import.Date


open System

type Date = TDate of string

let private dateFormat = "yyyy-MM-dd"

let private tryParseDate (d: string) =
    match d, DateTime.TryParse d with
    | "", _ -> None //fix JS parsing, it consumes everything and returns DateTime.Today
    | _, (true, _) -> TDate d |> Some
    | _, (false, _) -> None

let fromString d = tryParseDate d

let fromDateTime (d: DateTime) = d.ToString dateFormat |> TDate
let toDateTime (TDate  x) = x |> DateTime.Parse
let tryToDateTime (TDate  x) =
    match x |> DateTime.TryParse with
    | (true, d) -> Some d
    | (false, _) -> None

let value (TDate x) = x

let Today = DateTime.Today.ToString dateFormat |> TDate


let create(year, month, day) = DateTime(year, month, day) |> fromDateTime
