namespace Conway.App.Graphics

open Conway.App.Controls
open Conway.App.Input
open Conway.Core
open Raylib_cs
open System.Numerics

module Display =
    let posVec = Vector2(0.0f, 0.0f)

    let inline textureFlipRec width height =
        Rectangle(posVec, Vector2(width, -height))

    let init width height =
        // Raylib.SetConfigFlags ConfigFlags.ResizableWindow
        Raylib.SetTargetFPS 120
        Raylib.InitWindow(width, height, "Conway's game of life")

    let private renderBoardOnCanvas (canvas: Canvas) =
        let board = canvas.Game.CurrentState.Board
        let rows = Array2D.length1 board
        let cols = Array2D.length2 board

        let struct (visibleStartPoint, visibleEndPoint) = canvas.CalculateVisibleRange()
        let startRow = int visibleStartPoint.Y
        let startCol = int visibleStartPoint.X
        let endRow = max (min (int visibleEndPoint.Y) (rows - 2)) 1
        let endCol = max (min (int visibleEndPoint.X) (cols - 2)) 1

        let scaledCellWidth = canvas.CellSize * canvas.Camera.ZoomFactor
        let scaledCellHeight = canvas.CellSize * canvas.Camera.ZoomFactor

        let visibleCellSizeReciprocal = 1.0f / scaledCellHeight

        let halfWidth = canvas.Width * 0.5f * visibleCellSizeReciprocal
        let halfHeight = canvas.Height * 0.5f * visibleCellSizeReciprocal

        let upperLeftCornerX = canvas.Camera.Position.X - halfWidth
        let upperLeftCornerY = canvas.Camera.Position.Y - halfHeight

        for row = startRow to endRow do
            for col = startCol to endCol do
                let baseX = max (float32 col) visibleStartPoint.X - upperLeftCornerX
                let baseY = max (float32 row) visibleStartPoint.Y - upperLeftCornerY

                let trueX = canvas.CellSize + (baseX - 1.0f) * scaledCellWidth
                let trueY = canvas.CellSize + (baseY - 1.0f) * scaledCellHeight

                let actualEndX = (visibleEndPoint.X - upperLeftCornerX) * scaledCellWidth
                let actualEndY = (visibleEndPoint.Y - upperLeftCornerY) * scaledCellHeight

                let trueWidth = max (min scaledCellWidth (actualEndX - trueX)) 0.0f
                let trueHeight = max (min scaledCellHeight (actualEndY - trueY)) 0.0f

                if trueWidth = 0.0f || trueHeight = 0.0f then
                    ()
                else
                    match board[row, col] with
                    | 0<CellStatus> -> Draw.deadCell trueX trueY trueWidth trueHeight
                    | _ -> Draw.livingCell trueX trueY trueWidth trueHeight

    let private renderControls (controls: ControlManager) =
        for button in controls.Buttons do
            match button.IsVisible with
            | true -> Draw.button button
            | false -> ()

    let private renderGenerationCounter (canvas: Canvas) =
        Draw.textBox
            (canvas.Position.X + canvas.Width + 5.0f)
            canvas.Position.Y
            24
            $"Generation {canvas.Game.Generation}"

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

    let openFileDialogue (texture: RenderTexture2D) =
        let mutable isCancelled = false

        while not isCancelled do
            if Keyboard.keyHasBeenPressedOnce KeyboardKey.Escape then
                isCancelled <- true
            else
                Raylib.BeginTextureMode texture
                Raylib.ClearBackground Color.White

                Draw.textBox 10.0f 10.0f 50 "Bruh"

                Raylib.EndTextureMode()

                Raylib.BeginDrawing()

                Raylib.DrawTextureRec(
                    texture.Texture,
                    textureFlipRec (float32 texture.Texture.Width) (float32 texture.Texture.Height),
                    posVec,
                    Color.White
                )

                Raylib.EndDrawing()

    let mainWindow (controls: ControlManager) (texture: RenderTexture2D) fps mousePos =
        let canvas = controls.Canvas

        Raylib.BeginTextureMode texture
        Raylib.ClearBackground Color.White

        renderControls controls
        renderBoardOnCanvas canvas
        renderGenerationCounter canvas
        renderFpsCounter canvas fps
        renderMousePos canvas mousePos
        renderCanvasFocusCoordinates canvas

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
