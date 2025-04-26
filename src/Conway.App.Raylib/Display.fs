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
        // Raylib.DrawRectangle(canvas.X, canvas.Y, canvas.Width, canvas.Height, Color.Black)

        let rows = Array2D.length1 board
        let cols = Array2D.length2 board

        let struct (startX, startY, endX, endY) = canvas.CalculateVisibleRange()
        let startRow = int startY
        let startCol = int startX
        let endRow = max (min (int endY) (rows - 2)) 1
        let endCol = max (min (int endX) (cols - 2)) 1

        for row = startRow to endRow do
            for col = startCol to endCol do
                let trueX = float32 col + canvas.DrawingAreaX
                let trueY = float32 row + canvas.DrawingAreaY

                match board[row, col].Status with
                | Dead -> Draw.deadCell trueX trueY canvas.CellSize canvas.CellSize
                | Alive -> Draw.livingCell trueX trueY canvas.CellSize canvas.CellSize

    let private renderControls (controls: ControlManager) =
        for button in controls.Buttons do
            match button.IsVisible with
            | true -> Draw.button button
            | false -> ()

    let private renderGenerationCounter (canvas: Canvas) generation =
        Draw.textBox (canvas.X + canvas.Width + 5.0f) canvas.Y 24 $"Generation {generation}"

    let private renderFpsCounter (canvas: Canvas) fps =
        Draw.textBox (canvas.X + canvas.Width + 5.0f) (canvas.Y + 50.0f) 24 $"FPS {fps}"

    let private renderCanvasFocusCoordinates (canvas: Canvas) =
        Draw.textBox
            (canvas.X + canvas.Width + 5.0f)
            (canvas.Y + 100.0f)
            24
            $"Camera:\nX: {-canvas.DrawingAreaX:F2} Y: {-canvas.DrawingAreaY:F2}"

    let loadingScreen x y =
        for _ in 0..10 do
            Raylib.BeginDrawing()
            Raylib.ClearBackground Color.White

            Draw.textBox x y 48 "Loading..."

            Raylib.EndDrawing()

    let render (game: Game) (controls: ControlManager) texture fps =
        Raylib.BeginTextureMode texture
        Raylib.ClearBackground Color.Blank
        renderBoardOnCanvas controls.Canvas game.State.Board

        Raylib.EndTextureMode()

        Raylib.BeginDrawing()
        Raylib.ClearBackground Color.White

        renderControls controls
        renderGenerationCounter controls.Canvas game.Generation
        renderFpsCounter controls.Canvas fps
        renderCanvasFocusCoordinates controls.Canvas

        Raylib.DrawTextureRec(
            texture.Texture,
            textureFlipRec (float32 texture.Texture.Width) (float32 texture.Texture.Height),
            posVec,
            Color.White
        )

        Raylib.EndDrawing()

    let close () = Raylib.CloseWindow()
