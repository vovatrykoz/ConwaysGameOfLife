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
            Raylib.DrawRectangleLines(col * squareSize, row * squareSize, squareSize, squareSize, Color.Black)

            match playerCell.Status with
            | Dead ->
                Raylib.DrawRectangle(
                    col * squareSize + 5,
                    row * squareSize + 5,
                    squareSize - 10,
                    squareSize - 10,
                    Color.Black
                )
            | Alive ->
                Raylib.DrawRectangle(
                    col * squareSize + 5,
                    row * squareSize + 5,
                    squareSize - 10,
                    squareSize - 10,
                    Color.Red
                ))

    Raylib.EndDrawing()

Raylib.InitWindow(800, 600, "Conway's game of life")

render game.State.Board

let mutable hasRunOnce = false

while not (convertCBoolToFsBool (Raylib.WindowShouldClose())) do
    if not hasRunOnce then
        Thread.Sleep 1000
        hasRunOnce <- true
    else
        Thread.Sleep 250

    game.run (Limited 1)
    render game.State.Board

Raylib.CloseWindow()
