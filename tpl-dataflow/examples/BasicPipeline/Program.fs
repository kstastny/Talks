open System
open System.Threading.Tasks
open System.Threading.Tasks.Dataflow

open FSharp.Control.Tasks.V2

/// Recursive to make it slow on purpose
let rec fibRec n =
    match n with
    | 0 -> 0
    | 1 -> 1
    | _ -> fibRec (n-1) + fibRec (n-2)
    
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
    
let outputBlock = ActionBlock(fun x -> printfn "%s" x)        

/// Starts the pipeline and returns function that will post to the pipeline    
let startPipeline () =
    
    let debug x =
        let msg = sprintf "(debug) %A %s" DateTime.Now x
        //printfn "%s" msg
        outputBlock.Post msg |> ignore
        
        
    
    let blockOptions = ExecutionDataflowBlockOptions()
    blockOptions.MaxDegreeOfParallelism <- 1
    
    // Create dataflow block that will calculate the fibonacci numbers 
    let fibonacciBlock =
            TransformBlock<int, int64>(
                (fun n ->
                    task {
                        debug <| sprintf "Calculating fib%i" n
                        do! Task.Delay 500
                        let result = fib n
                        debug <| sprintf "Calculated fib%i, result is %i" n result
                        return result
                    }),
                blockOptions)
                
            
    // Create dataflow block that will take and output the result            
    let printBlock = ActionBlock(fun x -> printfn "Result is %A" x)
    
    // link the blocks together
    fibonacciBlock.LinkTo(printBlock) |> ignore
    
    // return function that will allow to post into the block
    fun n ->
        task {
            fibonacciBlock.SendAsync n |> ignore
        }
    
    
let rec nextCommand f =
    let cmd = Console.ReadLine ()
    match Int32.TryParse cmd with
    | (false, _) -> 0
    | (true, n) ->
        [1..n] |> List.map f |> ignore
        nextCommand f
                


[<EntryPoint>]
let main _ = 
    
    let postFib = startPipeline ()
    
    nextCommand postFib
    
     
