module NemeStats.Import.Http

open System
open System.Threading
open System.Net
open System.Net.Http
open System.IO
open System.Text

open FSharp.Control.Tasks.V2



type Authentication =
    | Anonymous
    | Basic of string * string   

type HttpError =
    | General of Exception
    | HttpRequestException of HttpRequestException
    | CallCanceled
    | HttpCallError of HttpStatusCode * reason:string * content:string
    with
        member x.Explain() =
            match x with
            | General e -> sprintf "An unexpected exception occured: %s" e.Message
            | HttpRequestException e -> sprintf "Error calling server: %s" e.Message
            | HttpCallError (status, message, _) ->  sprintf "Error calling server: %s: %s" (string status) message
            | CallCanceled -> sprintf "The operation has been canceled before it could complete"



let handler = new HttpClientHandler()
let httpClient = new HttpClient(handler)

let createRequest (method: HttpMethod) (uri:Uri) = 
    new HttpRequestMessage(method, uri)

let addHeader name (value: string) (httpRequestMessage: HttpRequestMessage) =
    httpRequestMessage.Headers.Add(name, value) |> ignore
    httpRequestMessage


let addAuthentication credentials (httpRequestMessage: HttpRequestMessage) =
    match credentials with
    | Anonymous -> httpRequestMessage
    | Basic (user, password) ->
        let headerValue = 
            sprintf "%s:%s" user password
            |> System.Text.Encoding.ASCII.GetBytes
            |> Convert.ToBase64String
            |> sprintf "Basic %s"
        addHeader "Authorization" headerValue httpRequestMessage


let setContent body contentType (requestMessage: HttpRequestMessage) =
    requestMessage.Content <- new StringContent(body, Encoding.UTF8, contentType)
    requestMessage


let getResponse (cancellationToken: CancellationToken) (requestMessage: HttpRequestMessage) =
    task {
        try
            let! r = httpClient.SendAsync(requestMessage, cancellationToken)

            use! stream = r.Content.ReadAsStreamAsync()
            use reader = new StreamReader(stream)
            let! responseContent = reader.ReadToEndAsync()    

            if r.IsSuccessStatusCode then           
                return responseContent |> Ok
            else
                return HttpCallError(r.StatusCode, r.ReasonPhrase, responseContent) |> Error
        with 
            | :? OperationCanceledException -> 
                return CallCanceled |> Error
            | :? HttpRequestException as ex ->
                return HttpRequestException ex |> Error
            | ex -> 
                return General ex |> Error                
    }


let uriJoin (uri: Uri) (append: string) =
    match append.Length with
    | 0 -> uri
    | _ ->
        match (uri.AbsoluteUri.[uri.AbsoluteUri.Length - 1], append.[0]) with
        | ('/', '/') -> Uri(uri.AbsoluteUri + append.Substring(1))
        | ('/', _)
        | _, '/' -> Uri(uri.AbsoluteUri + append)
        | _ -> Uri(uri.AbsoluteUri + "/" + append)


