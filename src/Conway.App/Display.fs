namespace Conway.App

open Conway.App.Controls
open Conway.Core
open Raylib_cs
open System.Numerics

module Display =
    let posVec = Vector2(0.0f, 0.0f)

    let textureFlipRec width height =
        Rectangle(posVec, Vector2(width, -height))

    let init width height =
        Raylib.SetConfigFlags ConfigFlags.ResizableWindow
        Raylib.SetTargetFPS 120
        Raylib.InitWindow(width, height, "Conway's game of life")

    let private renderBoardOnCanvas (canvas: Canvas) (board: int<CellStatus>[,]) =
        let rows = Array2D.length1 board
        let cols = Array2D.length2 board

        let struct (visibleStartPoint, visibleEndPoint) = canvas.CalculateVisibleRange()
        let startRow = int visibleStartPoint.Y
        let startCol = int visibleStartPoint.X
        let endRow = max (min (int visibleEndPoint.Y) (rows - 2)) 1
        let endCol = max (min (int visibleEndPoint.X) (cols - 2)) 1

        let trueWidth = canvas.CellSize * canvas.Camera.ZoomFactor
        let trueHeight = canvas.CellSize * canvas.Camera.ZoomFactor

        let visibleCellSizeReciprocal = 1.0f / trueHeight

        let halfWidth = canvas.Width * 0.5f * visibleCellSizeReciprocal
        let halfHeight = canvas.Height * 0.5f * visibleCellSizeReciprocal

        let upperLeftCornerX = canvas.Camera.Position.X - halfWidth
        let upperLeftCornerY = canvas.Camera.Position.Y - halfHeight

        for row = startRow to endRow do
            for col = startCol to endCol do
                let trueX = max (float32 col) visibleStartPoint.X - upperLeftCornerX
                let trueY = max (float32 row) visibleStartPoint.Y - upperLeftCornerY

                match board[row, col] with
                | 0<CellStatus> -> Draw.deadCell trueX trueY trueWidth trueHeight
                | _ -> Draw.livingCell trueX trueY trueWidth trueHeight

    let private renderControls (controls: ControlManager) =
        for button in controls.Buttons do
            match button.IsVisible with
            | true -> Draw.button button
            | false -> ()

    let private renderGenerationCounter (canvas: Canvas) generation =
        Draw.textBox (canvas.Position.X + canvas.Width + 5.0f) canvas.Position.Y 24 $"Generation {generation}"

    let private renderFpsCounter (canvas: Canvas) fps =
        Draw.textBox (canvas.Position.X + canvas.Width + 5.0f) (canvas.Position.Y + 50.0f) 24 $"FPS {fps}"

    let private renderMousePos (canvas: Canvas) (mousePos: Vector2) =
        Draw.textBox
            (canvas.Position.X + canvas.Width + 5.0f)
            (canvas.Position.Y + 170.0f)
            24
            $"Mouse:\nX {mousePos.X} Y {mousePos.Y}"

    let private renderCanvasFocusCoordinates (canvas: Canvas) =
        Draw.textBox
            (canvas.Position.X + canvas.Width + 5.0f)
            (canvas.Position.Y + 100.0f)
            24
            $"Camera:\nX: {canvas.Camera.Position.X:F2} Y: {canvas.Camera.Position.Y:F2}"

    let loadingScreen x y =
        for _ in 0..10 do
            Raylib.BeginDrawing()
            Raylib.ClearBackground Color.White

            Draw.textBox x y 48 "Loading..."

            Raylib.EndDrawing()

    let render (game: Game) (controls: ControlManager) (texture: RenderTexture2D) fps mousePos =
        Raylib.BeginTextureMode texture
        Raylib.ClearBackground Color.White

        renderBoardOnCanvas controls.Canvas game.CurrentState.Board
        renderControls controls
        renderGenerationCounter controls.Canvas game.Generation
        renderFpsCounter controls.Canvas fps
        renderMousePos controls.Canvas mousePos
        renderCanvasFocusCoordinates controls.Canvas

        Raylib.EndTextureMode()

        Raylib.BeginDrawing()

        Raylib.DrawTextureRec(
            texture.Texture,
            textureFlipRec (float32 texture.Texture.Width) (float32 texture.Texture.Height),
            posVec,
            Color.White
        )

        Raylib.EndDrawing()

    let inline close () = Raylib.CloseWindow()
