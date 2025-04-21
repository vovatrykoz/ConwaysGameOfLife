namespace Conway.App.Raylib

open Conway.Core
open Raylib_cs
open System.Numerics

module Display =
    let posVec = Vector2(0.0f, 0.0f)

    let textureFlipRec width height =
        Rectangle(posVec, Vector2(width, -height))

    let init width height =
        Raylib.SetConfigFlags ConfigFlags.FullscreenMode
        Raylib.InitWindow(width, height, "Conway's game of life")

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

    let private renderGenerationCounter generation =
        Draw.textBox 1680 50 24 $"Generation {generation}"

    let render (game: Game) (controls: ControlManager) texture =
        Raylib.BeginTextureMode texture
        Raylib.ClearBackground Color.Blank
        renderBoardOnCanvas controls.Canvas (ConwayGrid.board game.State)

        Raylib.EndTextureMode()

        Raylib.BeginDrawing()
        Raylib.ClearBackground Color.White

        renderControls controls
        renderGenerationCounter game.Generation

        Raylib.DrawTextureRec(
            texture.Texture,
            textureFlipRec (float32 texture.Texture.Width) (float32 texture.Texture.Height),
            posVec,
            Color.White
        )

        Raylib.EndDrawing()

    let close () = Raylib.CloseWindow()
