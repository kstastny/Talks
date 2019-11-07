open System
open System.Collections.Generic

open System.Threading.Tasks
open System.Threading.Tasks.Dataflow

open System.Collections.Concurrent
open FSharp.Control.Tasks.V2

let fib n =    
    match n with
    | 0 -> 0L
    | 1 -> 1L
    | _ ->
        Seq.unfold(fun (prevFib, currentFib) ->
            let nextFib = prevFib + currentFib
            Some(nextFib, (currentFib, nextFib))
            )
            (0L,1L)
        |> Seq.skip(n-2)
        |> Seq.head


        
let fibonacciBlock =
    let blockOptions = ExecutionDataflowBlockOptions()
    blockOptions.MaxDegreeOfParallelism <- 2
    blockOptions.BoundedCapacity <- 5

    ActionBlock<int>(
        (fun n ->
            task {
                do! Task.Delay 250
                printfn "Fibonacci %i is %i" n (fib n)
            } |> Async.AwaitTask |> Async.RunSynchronously),
        blockOptions)        
        
        
let loadSheddingPipeline =
    
    let broadcastBlock = BroadcastBlock(fun x -> x)
    
    broadcastBlock.LinkTo(fibonacciBlock) |> ignore
    
    broadcastBlock

let rec postWithBackPressure (block: ITargetBlock<int>) n =
    async {
        if block.Post(n) then
            printfn "Enqueued fib %i" n
            return ()
        else
            printfn "Buffer full, wait with further work, cannot assign %i" n
            do! Async.Sleep 1000
            return! postWithBackPressure block n
    }


[<EntryPoint>]
let main _ =
    
    [1..10]
    |> List.map (postWithBackPressure fibonacciBlock)
    //|> List.map (postWithBackPressure loadSheddingPipeline)
    |> List.iter (fun n -> n |> Async.RunSynchronously)
    
    fibonacciBlock.Complete()
    
    task { do! fibonacciBlock.Completion }
    |> Async.AwaitTask
    |> Async.RunSynchronously
    
    0
    
