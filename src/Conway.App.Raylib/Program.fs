open Conway.Core
open Conway.App.Raylib
open Raylib_cs
open System.Threading

let convertCBoolToFsBool (cbool: CBool) =
    match sbyte cbool with
    | 0y -> false
    | _ -> true

let startingState = Preset.presetOne |||> Grid.init
let squareSize = 25

let game = new Game(startingState)

let render board =
    Raylib.BeginDrawing()
    Raylib.ClearBackground Color.White

    board
    |> Array2D.iteri (fun row col cell ->
        match cell with
        | BorderCell -> ()
        | PlayerCell playerCell ->
            match playerCell.Status with
            | Dead -> Draw.deadCell col row
            | Alive -> Draw.livingCell col row)

    Raylib.EndDrawing()

Raylib.InitWindow(800, 600, "Conway's game of life")

render game.State.Board

let mutable hasRunOnce = false

while not (convertCBoolToFsBool (Raylib.WindowShouldClose())) do
    if not hasRunOnce then
        Thread.Sleep 1000
        hasRunOnce <- true
    else
        Thread.Sleep 500

    game.run (Limited 1)
    render game.State.Board

Raylib.CloseWindow()
