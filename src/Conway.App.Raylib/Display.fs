namespace Conway.App.Raylib

open Conway.Core
open Raylib_cs

module Display =
    let init () =
        Raylib.InitWindow(800, 600, "Conway's game of life")

    let private renderBoardOnCanvas (canvas: Canvas) board =
        board
        |> Array2D.iteri (fun row col cell ->
            match cell with
            | BorderCell -> ()
            | PlayerCell playerCell ->
                match playerCell.Status with
                | Dead ->
                    Draw.deadCell
                        (col + canvas.DrawingAreaX)
                        (row + canvas.DrawingAreaY)
                        canvas.BaseCellSize
                        canvas.BaseCellSize
                | Alive ->
                    Draw.livingCell
                        (col + canvas.DrawingAreaX)
                        (row + canvas.DrawingAreaY)
                        canvas.BaseCellSize
                        canvas.BaseCellSize)

    let private renderControls (controls: ControlManager) =
        controls.Buttons
        |> Seq.iter (fun button ->
            match button.IsVisible with
            | true -> Draw.button button
            | false -> ())

    let private renderGenerationCounter generation = Draw.textBox $"Generation {generation}"

    let render (game: Game) (controls: ControlManager) =
        Raylib.BeginDrawing()
        Raylib.ClearBackground Color.White

        renderBoardOnCanvas controls.Canvas game.State.Board
        renderControls controls
        renderGenerationCounter game.Generation

        Raylib.EndDrawing()

    let close () = Raylib.CloseWindow()
