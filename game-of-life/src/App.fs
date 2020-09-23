module App

open Browser.Dom

open System

open Fable.Core.JsInterop

module GameOfLife =

    type Cell =
        | Alive of int * int
        | Dead of int * int

    type World =
        | World of Cell array array


    let map (f: Cell -> Cell) (World cells) : World =
        cells 
            |> Array.map (fun row -> row |> Array.map f)
            |> World


    let iter (f: Cell -> unit) (World cells) : unit =
        cells 
            |> Array.iter (fun row -> row |> Array.iter f)
            


    let liveNeighbourCount (World cells) (c: Cell) : int =
        let i,j =
            match c with
            | Alive (i,j) -> i,j
            | Dead (i,j) -> i,j
        [ (i-1, j-1) ; (i-1, j) ; (i-1, j+1);
        (i, j-1); (i, j+1);
        (i+1, j-1); (i+1, j); (i+1, j+1) ]
        |> List.where (fun (x,y) ->
            x >= 0 && y >= 0 && x < cells.Length && y < cells.[0].Length
        )
        |> List.map (fun (x,y) ->
            cells.[x].[y]
        )
        |> List.where (fun x ->
            match x with 
            | Alive _ -> true
            | Dead _ -> false)
        |> List.length



    // 1. Alive - if it has two or three neighbors -> Alive
    // 2. Dead and has three neighbors -> Alive
    // 3. otherwise -> Dead

    let step (w: World) : World =
        w |> map (fun cell ->
            match (cell, liveNeighbourCount w cell) with
            | (Alive (i,j), n) when n = 2 || n = 3 -> Alive (i,j)
            | (Dead  (i,j), 3) -> Alive (i,j)
            | Alive(i,j), _ -> Dead (i,j)
            | Dead(i,j), _ -> Dead (i,j)
            )

    let rand = Random()

    let generate size =
        [|
            for i in [0..size - 1] do
                [|
                    for j in [0..size - 1] do
                        match rand.NextDouble() with
                        | n when n < 0.25 -> Alive(i,j)
                        | _ -> Dead(i,j)
                |]
        |]
        |> World


open GameOfLife

let mutable world = generate 50

let canvas = document.querySelector("#canvas") :?> Browser.Types.HTMLCanvasElement

let ctx = canvas.getContext_2d ()

let size = 10.

let displayWorld (w: World) =
    w
     |> iter (fun cell ->
        match cell with
        | Alive(i, j) -> 
            ctx.fillStyle <- !^ "#000000"
            ctx.fillRect(float i * size, float j * size, size, size)
        | Dead(i, j) -> 
            ctx.fillStyle <- !^ "#FFFFFF"
            ctx.fillRect(float i * size, float j * size, size, size)        
     )

displayWorld world

// Get a reference to our button and cast the Element to an HTMLButtonElement
let stepButton = document.querySelector("#step") :?> Browser.Types.HTMLButtonElement

// Register our listener
stepButton.onclick <- fun _ ->
    world <- step world
    displayWorld world


