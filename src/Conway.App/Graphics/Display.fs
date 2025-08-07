namespace Conway.App.Graphics

open Conway.App
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

        let scaledCellSize = canvas.CellSize * canvas.Camera.ZoomFactor
        let scaledCellSizeReciprocal = 1.0f / scaledCellSize

        let halfWidth = canvas.Width * 0.5f * scaledCellSizeReciprocal
        let halfHeight = canvas.Height * 0.5f * scaledCellSizeReciprocal

        let upperLeftCornerX = canvas.Camera.Position.X - halfWidth
        let upperLeftCornerY = canvas.Camera.Position.Y - halfHeight

        for row = startRow to endRow do
            for col = startCol to endCol do
                let baseX = max (float32 col) visibleStartPoint.X - upperLeftCornerX
                let baseY = max (float32 row) visibleStartPoint.Y - upperLeftCornerY

                let trueX = canvas.CellSize + (baseX - 1.0f) * scaledCellSize
                let trueY = canvas.CellSize + (baseY - 1.0f) * scaledCellSize

                let actualEndX = (visibleEndPoint.X - upperLeftCornerX) * scaledCellSize
                let actualEndY = (visibleEndPoint.Y - upperLeftCornerY) * scaledCellSize

                let trueWidth = max (min scaledCellSize (actualEndX - trueX)) 0.0f
                let trueHeight = max (min scaledCellSize (actualEndY - trueY)) 0.0f

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
        Draw.label
            (canvas.Position.X + canvas.Width + 5.0f)
            canvas.Position.Y
            24
            $"Generation {canvas.Game.Generation}"
            30
            30
            Color.Black
            Color.White

    let private renderFpsCounter (canvas: Canvas) fps =
        Draw.label
            (canvas.Position.X + canvas.Width + 5.0f)
            (canvas.Position.Y + 50.0f)
            24
            $"FPS {fps}"
            30
            30
            Color.Black
            Color.RayWhite

    let private renderMousePos (canvas: Canvas) (mousePos: Vector2) =
        Draw.label
            (canvas.Position.X + canvas.Width + 5.0f)
            (canvas.Position.Y + 170.0f)
            24
            $"Mouse:\nX {mousePos.X} Y {mousePos.Y}"
            30
            30
            Color.Black
            Color.RayWhite

    let private renderCanvasFocusCoordinates (canvas: Canvas) =
        Draw.label
            (canvas.Position.X + canvas.Width + 5.0f)
            (canvas.Position.Y + 100.0f)
            24
            $"Camera:\nX: {canvas.Camera.Position.X:F2} Y: {canvas.Camera.Position.Y:F2}"
            30
            30
            Color.Black
            Color.RayWhite

    let loadingScreen x y =
        for _ in 0..10 do
            Raylib.BeginDrawing()
            Raylib.ClearBackground Color.White

            Draw.label x y 48 "Loading..." 50 50 Color.Black Color.RayWhite

            Raylib.EndDrawing()

    let saveFileDialogue (texture: RenderTexture2D) (fileSaver: FileSaver) =

        Raylib.BeginTextureMode texture
        Raylib.ClearBackground Color.White

        Draw.label
            (float32 fileSaver.X)
            (float32 10.0f + float32 fileSaver.FileEntryHeight)
            (int (fileSaver.FileEntryHeight - 10.0f))
            (fileSaver.Buffer.ToString())
            (int fileSaver.FileEntryWidth)
            (int fileSaver.FileEntryHeight)
            Color.White
            Color.Black

        Draw.button fileSaver.ConfirmButton
        Draw.button fileSaver.CancelButton

        Raylib.EndTextureMode()

        Raylib.BeginDrawing()

        Raylib.DrawTextureRec(
            texture.Texture,
            textureFlipRec (float32 texture.Texture.Width) (float32 texture.Texture.Height),
            posVec,
            Color.White
        )

        Raylib.EndDrawing()

    let openFileDialogue (texture: RenderTexture2D) (filePicker: FilePicker) =
        let struct (startIndex, endIndex) = filePicker.CalculateVisibleIndexRange()

        Raylib.BeginTextureMode texture
        Raylib.ClearBackground Color.White

        for index = startIndex to endIndex do
            let currentFile = filePicker.Files.[index]

            let currentItemIsSelected =
                match filePicker.CurrentSelection with
                | None -> false
                | Some file -> file = currentFile

            let y = max filePicker.Y -filePicker.Camera.Position.Y

            if currentItemIsSelected then
                Draw.label
                    (float32 filePicker.X)
                    (float32 y + float32 filePicker.FileEntryHeight * float32 (index - startIndex))
                    (int (filePicker.FileEntryHeight - 10.0f))
                    currentFile.Name
                    (int filePicker.FileEntryWidth)
                    (int filePicker.FileEntryHeight)
                    Color.White
                    Color.Black
            else
                Draw.label
                    (float32 filePicker.X)
                    (float32 y + float32 filePicker.FileEntryHeight * float32 (index - startIndex))
                    (int (filePicker.FileEntryHeight - 10.0f))
                    currentFile.Name
                    (int filePicker.FileEntryWidth)
                    (int filePicker.FileEntryHeight)
                    Color.Black
                    Color.White

        Draw.button filePicker.ConfirmButton
        Draw.button filePicker.CancelButton

        Raylib.EndTextureMode()

        Raylib.BeginDrawing()

        Raylib.DrawTextureRec(
            texture.Texture,
            textureFlipRec (float32 texture.Texture.Width) (float32 texture.Texture.Height),
            posVec,
            Color.White
        )

        Raylib.EndDrawing()

    let mainWindow (controls: ControlManager) (canvas: Canvas) (texture: RenderTexture2D) fps mousePos =
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
