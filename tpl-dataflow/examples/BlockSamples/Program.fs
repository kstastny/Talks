open System
open System.Collections.Generic
open System.Threading.Tasks
open System.Threading.Tasks.Dataflow

open System.Threading.Tasks.Dataflow
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


// https://en.wikipedia.org/wiki/Triangular_number
let triangular n =  (n * (n+1)) / 2

// Lucas numbers https://www.universityherald.com/articles/54943/20161213/3-more-amazing-math-sequences-beyond-fibonacci.htm
let lucas n =
    match n with
    | 0 -> 2L
    | 1 -> 1L
    | _ ->
        Seq.unfold(fun (prev, curr) ->
            let next = prev + curr
            Some(next, (curr, next))
            )
            (2L,1L)
        |> Seq.skip(n-2)
        |> Seq.head    


// Perrin Skiponacci https://www.universityherald.com/articles/54943/20161213/3-more-amazing-math-sequences-beyond-fibonacci.htm
let perrin n =
    match n with
    | 0 -> 3L
    | 1 -> 0L
    | 2 -> 2L
    | _ ->
        Seq.unfold(fun (prev2, prev, curr) ->
            let next = prev2 + prev
            Some(next, (prev, curr, next))
            )
            (3L,0L,2L)
        |> Seq.skip(n-3)
        |> Seq.head      




type Fibonacci = Fibonacci of int * int64
type Perrin = Perrin of int
type Lucas = Lucas of int * int64

type AggregateData =
    {
        Fib : Fibonacci option
        Lucas: Lucas option
    }

let pipeline =
    
    let blockOptions = ExecutionDataflowBlockOptions()
    blockOptions.MaxDegreeOfParallelism <- 2
    blockOptions.BoundedCapacity <- 50
    blockOptions.EnsureOrdered <- false
    
    let broadcastBlock = BroadcastBlock(fun x -> x)
    
    let fibonacciBlock =
        TransformBlock<int, Fibonacci>(
            (fun n ->
                task {
                    do! Task.Delay 80
                    return Fibonacci(n, (fib n))
                }),
            blockOptions)
        
    let lucasBlock =
        TransformBlock<int, Lucas>(
            (fun n ->
                task {
                    if n % 2 = 0 then
                        printfn "asdf"
                        do! Task.Delay 1000
                    else
                        do! Task.Delay 20
                    return Lucas(n, (lucas n))
                }),
            blockOptions)
        
        
    let groupingOpts = new GroupingDataflowBlockOptions()
    //NOTE: if not greedy, the JoinBlock will only accept data if it can form a tuple. Otherwise it will accept data always
    groupingOpts.Greedy <- false
    
        
    let joinBlock = JoinBlock<Fibonacci, Lucas>(groupingOpts)
        
    let aggregateState = new Dictionary<int, AggregateData>();
    let aggregateBlock = ActionBlock<Tuple<Fibonacci, Lucas>>(fun tup -> printfn "%A" tup)
        

    broadcastBlock.LinkTo(fibonacciBlock) |> ignore
    broadcastBlock.LinkTo(lucasBlock) |> ignore
    
    
    fibonacciBlock.LinkTo(joinBlock.Target1) |> ignore
    lucasBlock.LinkTo(joinBlock.Target2) |> ignore
    
    joinBlock.LinkTo(aggregateBlock) |> ignore    
    
    broadcastBlock


let rec nextCommand f =
    printf "How many items do you want? "
    let cmd = Console.ReadLine ()
    match Int32.TryParse cmd with
    | (false, _) -> 0
    | (true, n) ->
        [1..n] |> List.map f |> ignore
        nextCommand f


[<EntryPoint>]
let main _ =
    
    nextCommand (pipeline.Post)
    
