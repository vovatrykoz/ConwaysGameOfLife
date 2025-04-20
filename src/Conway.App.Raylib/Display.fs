namespace Conway.App.Raylib

open Conway.Core
open Raylib_cs

module Display =
    let init () =
        Raylib.InitWindow(800, 600, "Conway's game of life")

    let private renderBoardOnCanvas (canvas: Canvas) board =
        Raylib.DrawRectangleLinesEx(
            new Rectangle(float32 canvas.X, float32 canvas.Y, float32 canvas.Width, float32 canvas.Height),
            2.0f,
            Color.Black
        )

        board
        |> Array2D.iteri (fun row col cell ->
            match cell with
            | BorderCell -> ()
            | PlayerCell playerCell ->
                let trueX = col + canvas.DrawingAreaX
                let trueY = row + canvas.DrawingAreaY

                if
                    trueX * canvas.BaseCellSize < canvas.X
                    || trueX * canvas.BaseCellSize >= canvas.X + canvas.Width
                    || trueY * canvas.BaseCellSize < canvas.Y
                    || trueY * canvas.BaseCellSize >= canvas.Y + canvas.Height
                then
                    ()
                else
                    match playerCell.Status with
                    | Dead -> Draw.deadCell trueX trueY canvas.BaseCellSize canvas.BaseCellSize
                    | Alive -> Draw.livingCell trueX trueY canvas.BaseCellSize canvas.BaseCellSize)

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

        renderBoardOnCanvas controls.Canvas (ConwayGrid.board game.State)
        renderControls controls
        renderGenerationCounter game.Generation

        Raylib.EndDrawing()

    let close () = Raylib.CloseWindow()
