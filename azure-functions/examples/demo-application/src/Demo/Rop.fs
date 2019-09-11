module FSharp.Control.Rop

let bind2 f x y =
    match x, y with
    | Ok xR, Ok yR -> f xR yR
    | Error e, _ | _, Error e -> Error e

let apply resultF result =
    match resultF with
    | Ok f -> Result.map f result
    | Error e -> Error e

//https://stackoverflow.com/questions/50789065/turn-list-of-result-into-result-of-list-inside-a-computation-expression
// and https://fsharpforfunandprofit.com/posts/elevated-world-4/
let sequence results =
    let foldFn item acc =
        match acc, item with
        | Error e, _ | _, Error e -> Error e
        | Ok l, Ok v -> v :: l |> Ok
    List.foldBack foldFn results (Ok [])
    
    
// http://hackage.haskell.org/package/base-4.12.0.0/docs/Data-Bitraversable.html is the same?
let bisequence results =     
    let foldFn item (acc: Result<'a list, 'e list>) =
        match acc, item with
        | Ok l, Ok v -> v :: l |> Ok
        | Ok _, Error e -> Error [ e ]
        | Error e1, Ok _ -> Error e1
        | Error e1, Error e2 -> Error (e2 :: e1)
    List.foldBack foldFn results (Ok [])

let bindOption e opt =
    match opt with
    | Some(x) -> Ok x
    | None -> Error e

let extractOrNone = function
    | Ok x -> Some x
    | Error _ -> None

let isOk = function
    | Ok(_) -> true
    | _ -> false

let resultGet x =
    match x with
    | Ok xR -> xR
    | Error e -> failwith <| sprintf "%A" e

let errorGet x =
    match x with
    | Ok _ -> failwith "Cannot get error from Ok case"
    | Error e -> e

let flatten = function
    | Ok tr -> tr
    | Error e -> Error e


///https://fsharpforfunandprofit.com/posts/elevated-world-4/#traverse (monadic style)
let rec traverse f list =

    // define the monadic functions
    let (>>=) x f = Result.bind f x
    let retn = Ok

    // loop through the list
    match list with
    | [] ->
        // if empty, lift [] to a Result
        retn []
    | head::tail ->
        // otherwise lift the head to a Result using f
        // then lift the tail to a Result using traverse
        // then cons the head and tail and return it
        f head                 >>= (fun h ->
        traverse f tail >>= (fun t ->
        retn (h :: t) ))


type ResultBuilder() =
    member __.Zero() = Ok()
    member __.Bind(m, f) = Result.bind f m
    member __.Return(x) = Ok x
    member __.ReturnFrom(x) = x
    member __.Combine (a, b) = Result.bind b a
    member __.Delay f = f
    member __.Run f = f ()
    member __.TryWith (body, handler) =
        try
            body()
        with
        | e -> handler e
    member __.TryFinally (body, compensation) =
        try
            body()
        finally
            compensation()
    member x.Using(d:#System.IDisposable, body) =
        let result = fun () -> body d
        x.TryFinally (result, fun () ->
            match d with
            | null -> ()
            | d -> d.Dispose())
    member x.While (guard, body) =
        if not <| guard () then
            x.Zero()
        else
            Result.bind (fun () -> x.While(guard, body)) (body())
    member x.For(s:seq<_>, body) =
        x.Using(s.GetEnumerator(), fun enum ->
            x.While(enum.MoveNext,
                x.Delay(fun () -> body enum.Current)))

let result = ResultBuilder()

let (>>=) result f = Result.bind f result
let (<!>) result f = Result.map f result
let (<*>) = apply

module Async =


    let bind (f:'a -> Async<Result<'b,'c>>) result =
        async {
            let! res = result
            match res with
            | Ok s -> return! (f s)
            | Error f -> return Error f
        }

    let mapError f inp =
        async {
            let! res = inp
            match res with
            | Ok x -> return Ok x
            | Error e -> return Error (f e)
        }

    let apply resultF result =
        async {
            let! resF = resultF
            let! res = result
            return resF <*> res
        }

    let bind2 f x y =
        async {
            let! xR = x
            let! yR = y
            match xR, yR with
            | Ok xR, Ok yR -> return! f xR yR
            | Error e, _ | _, Error e -> return Error e
        }

    let (>>=) result f = bind f result
    let (<!>) result f = bind (f >> Ok >> async.Return) result
    let (<*>) = apply


module TaskResult =

    open System
    open System.Threading.Tasks

    open FSharp.Control.Tasks.V2


    let fromResult (r:Result<'a,'b>) = Task.FromResult r

    let retn x = x |> Ok |> fromResult

    let bind (f:'a -> Task<Result<'b,'c>>) (taskResult:Task<Result<'a,'c>>) =
        task {
            let! res = taskResult
            match res with
            | Ok s -> return! (f s)
            | Error f -> return Error f
        }

    let map (f:'a -> 'b) (taskResult:Task<Result<'a,'c>>) =
        task {
            let! res = taskResult
            match res with
            | Ok s -> return (f s) |> Ok
            | Error f -> return Error f
        }

    let bindResult (f: 'a -> Task<Result<'b, 'c>>) result =
        task {
            match result with
            | Ok s -> return! (f s)
            | Error f -> return Error f
        }

    let mapError (f:'b -> 'c) (inp:Task<Result<'a,'b>>) =
        task {
            let! res = inp
            match res with
            | Ok x -> return Ok x
            | Error e -> return Error (f e)
        }

    let apply (resultF:Task<Result<('a -> 'b), 'c>>) (result:Task<Result<'a,'c>>) =
        task {
            let! resF = resultF
            let! res = result
            return resF <*> res


        }

    let sequence (l: Task<Result<'a, 'b>> list) : Task<Result<'a list, 'b>> =
        task {
            Task.WaitAll( l |> Seq.cast<Task> |> Array.ofSeq)
            
            return l |> List.map (fun y -> y.Result) |> sequence
        }
        
    let bisequence (l: Task<Result<'a, 'b>> list) : Task<Result<'a list, 'b list>> =
        task {
            Task.WaitAll( l |> Seq.cast<Task> |> Array.ofSeq)
            
            return l |> List.map (fun y -> y.Result) |> bisequence
        }        
    
    
    let bind2 (f:'a -> 'b -> Task<Result<'c,'d>>) (x:Task<Result<'a,'d>>) (y:Task<Result<'b,'d>>) =
        task {
            let! xR = x
            let! yR = y
            match xR, yR with
            | Ok xR, Ok yR -> return! f xR yR
            | Error e, _ | _, Error e -> return Error e
        }


    let delay (f: unit -> Task<Result<'a, 'b>>) = f ()

    let combine (x: Task<Result<'a, 'b>>) (y: Task<Result<'a, 'b>>) : Task<Result<'a, 'b>> =
        task {
            let! xR = x
            match xR with
            | Ok _ -> return! y
            | Error e -> return e |> Error
        }


    let tryFinally (expr: Task<Result<'a, 'b>>) (compensation: unit -> unit) =
        task {
            try
                return! expr
            finally
                compensation ()
        }

    let tryWith (expr: Task<Result<'a, 'b>>) handler =
        task {
            try
                return! expr
            with
            | ex -> return! handler ex
        }

    let using (resource: #System.IDisposable) func =
        tryFinally
            (func resource)
            (fun () -> resource.Dispose())


    let (>>=) result f = bind f result
    let (<!>) result f = bind (f >> Ok >> Task.FromResult) result
    let (<*>) = apply

    //https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/computation-expressions
    //https://github.com/rspeele/TaskBuilder.fs/blob/master/TaskBuilder.fs
    //http://www.fssnip.net/7UJ/title/ResultBuilder-Computational-Expression
    type TaskResultBuilder() =


        //M<'T> * ('T -> M<'U>) -> M<'U>
        member __.Bind(m, f) = bind f m


        //(unit -> M<'T>) -> M<'T>
        member __.Delay f = delay f

        //'T -> M<'T>
        member __.Return x = retn x

        //M<'T> -> M<'T>
        member __.ReturnFrom (x: Task<Result<'a, 'b>>) = x


        //(M<'T> -> M<'T>) or (M<'T> -> 'T)
        member __.Run (f : Task<Result<'a, 'b>>) = f

        //(M<'T> * M<'T> -> M<'T>) or (M<unit> * M<'T> -> M<'T>)

        member __.Combine(x, y) = combine x y

        // (M<'T> * (unit -> unit) -> M<'T>)
        member __.TryFinally(expr, compensation) = tryFinally expr compensation

        // 'T * ('T -> M<'U>) -> M<'U> when 'U :> IDisposable
        member __.Using(res:#IDisposable, body) = using res body

        //M<'T> * (exn -> M<'T>) -> M<'T>
        member __.TryWith(expr, handler) = tryWith expr handler

        //unit -> M<'T>
        member __.Zero () = () |> retn


    let taskResult = TaskResultBuilder()