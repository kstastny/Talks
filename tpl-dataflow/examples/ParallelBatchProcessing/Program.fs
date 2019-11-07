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
        
        
let rec nextCommand f =
    printf "How many Fibonacci numbers do you want to calculate? "
    let cmd = Console.ReadLine ()
    match Int32.TryParse cmd with
    | (false, _) -> 0
    | (true, n) ->
        task {
            let! x = f n            
            printfn "Result: %A" x 
        } |> Async.AwaitTask |> Async.RunSynchronously
        nextCommand f
        
        
let postFib n =
    
    task {
        try
            // calculate fibonaccis in parallel and write them out when we have all of them. This will also stop the pipeline
            let blockOptions = ExecutionDataflowBlockOptions()
            blockOptions.MaxDegreeOfParallelism <- 5
            
            // using List is ok since Rx.net runs as singlethreaded by default
            let resultList = List<int * int64>()
            // Create dataflow block that will calculate the fibonacci numbers 
            let fibonacciBlock =
                    TransformBlock<int, int * int64>(
                        (fun n ->
                            task {
                                do! Task.Delay 100

                                //NOTE: uncomment for error demo                                
//                                if n = 5 then
//                                    failwithf "Number 5 is prohibited."
                                
                                return (n, fib n)
                            }),
                        blockOptions)
                
            // TODO maybe keep this for different example and use ActionBlock?                
            let observer = fibonacciBlock.AsObservable().Subscribe(fun x -> resultList.Add(x))                
            
            { 0 .. n } |> Seq.iter (fun i -> fibonacciBlock.Post(i) |> ignore)
            
            // indicate that no more values will be pushed into the block
            fibonacciBlock.Complete()
            
            // wait for calculations to finish
            do! fibonacciBlock.Completion
            observer.Dispose ()
            
            return resultList |> List.ofSeq
        with
            ex ->
                printfn "Error calculating fibs: %A" ex
                return [] //NOTE: just for demonstration purposes we will ignore the error
    }
    
// alternative implementation of Parallel batch    
let postFib2 n =
    
    task {
        try
            // calculate fibonaccis in parallel and write them out when we have all of them. This will also stop the pipeline
            let blockOptions = ExecutionDataflowBlockOptions()
            blockOptions.MaxDegreeOfParallelism <- 5
            
            let results = ConcurrentQueue<int * int64>()
            // Create dataflow block that will calculate the fibonacci numbers 
            let fibonacciBlock =
                    ActionBlock<int>(
                        (fun n ->
                            task {
                                do! Task.Delay 100
                                results.Enqueue(n, fib n)
                            } |> Async.AwaitTask |> Async.RunSynchronously),
                        blockOptions)
                
            
            { 0 .. n } |> Seq.iter (fun i -> fibonacciBlock.Post(i) |> ignore)

            // indicate that no more values will be pushed into the block            
            fibonacciBlock.Complete()
            
            // wait for calculations to finish            
            do! fibonacciBlock.Completion
            
            return results |> List.ofSeq
        with
            ex ->
                printfn "Error calculating fibs: %A" ex
                return [] //NOTE: just for demonstration purposes we will ignore the error
    }    
    
    
[<EntryPoint>]
let main _ =
    
    nextCommand postFib2
