namespace NemeStats.Import.FunctionApp

open System.IO
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Host
open Microsoft.Extensions.Logging

module BlobTrigger1 =
    [<FunctionName("BlobTrigger1")>]
    //Connection: default is `AzureWebJobsStorage`
    let run ([<BlobTrigger("fff/{name}", Connection = "AzureWebJobsStorage")>] myBlob: Stream, name: string, log: ILogger) =
        printfn "HERE"
        let msg = sprintf "F# Blob trigger function Processed blob\nName: %s \n Size: %d Bytes" name myBlob.Length
        log.LogInformation msg