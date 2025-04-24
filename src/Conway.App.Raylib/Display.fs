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

    let private renderBoardOnCanvas (canvas: Canvas) (board: Cell[,]) =
        Raylib.DrawRectangleLinesEx(
            Rectangle(float32 canvas.X, float32 canvas.Y, float32 canvas.Width, float32 canvas.Height),
            2.0f,
            Color.Black
        )

        let rows = Array2D.length1 board
        let cols = Array2D.length2 board

        let struct (startX, startY, endX, endY) = canvas.CalculateVisibleRange()
        let adjustedEndX = max (min endX (cols - 2)) 1
        let adjustedEndY = max (min endY (rows - 2)) 1

        for row = startY to adjustedEndY do
            for col = startX to adjustedEndX do
                let trueX = col + canvas.DrawingAreaX
                let trueY = row + canvas.DrawingAreaY

                match board[row, col].Status with
                | Dead -> Draw.deadCell trueX trueY canvas.CellSize canvas.CellSize
                | Alive -> Draw.livingCell trueX trueY canvas.CellSize canvas.CellSize

    let private renderControls (controls: ControlManager) =
        for button in controls.Buttons do
            match button.IsVisible with
            | true -> Draw.button button
            | false -> ()

    let private renderGenerationCounter generation =
        Draw.textBox 1680 50 24 $"Generation {generation}"

    let loadingScreen x y =
        for _ in 0..10 do
            Raylib.BeginDrawing()
            Raylib.ClearBackground Color.White

            Draw.textBox x y 48 "Loading..."

            Raylib.EndDrawing()

    let render (game: Game) (controls: ControlManager) texture =
        Raylib.BeginTextureMode texture
        Raylib.ClearBackground Color.Blank
        renderBoardOnCanvas controls.Canvas game.State.Board

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
