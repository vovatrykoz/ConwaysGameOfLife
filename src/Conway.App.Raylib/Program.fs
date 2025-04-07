open Conway.Core
open Raylib_cs
open System.Threading

let convertCBoolToFsBool (cbool: CBool) =
    match sbyte cbool with
    | 0y -> false
    | _ -> true

let startingArray = [|
    [| Cell.dead; Cell.dead; Cell.dead; Cell.dead; Cell.dead |]
    [| Cell.dead; Cell.dead; Cell.dead; Cell.dead; Cell.dead |]
    [| Cell.dead; Cell.living; Cell.living; Cell.living; Cell.dead |]
    [| Cell.dead; Cell.dead; Cell.dead; Cell.dead; Cell.dead |]
    [| Cell.dead; Cell.dead; Cell.dead; Cell.dead; Cell.dead |]
|]

let initializer i j = startingArray[i][j]

let startingState = Grid.init 5 5 initializer
let squareSize = 100

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

while not (convertCBoolToFsBool (Raylib.WindowShouldClose())) do
    Thread.Sleep 500
    game.run (Limited 1)
    render game.State.Board

Raylib.CloseWindow()
