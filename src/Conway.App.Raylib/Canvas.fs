namespace Conway.App.Raylib

open Conway.Core
open Raylib_cs
open System.Numerics

type Canvas
    (
        x: float32,
        y: float32,
        width: float32,
        height: float32,
        drawingX: float32,
        drawingY: float32,
        game: Game,
        cellSize: float32
    ) =

    member val Position = Vector2(x, y) with get, set

    member val Width = width with get, set

    member val Height = height with get, set

    member val CellSize = cellSize with get, set

    member val Game = game with get, set

    member val Camera = Camera(-drawingX, -drawingY) with get, set

    member this.CalculateVisibleRange() =
        let cellSize = this.CellSize * this.Camera.ZoomFactor

        let offset = this.Camera.Position * cellSize

        let activeWidth = min ((this.Width - offset.X) / cellSize) (this.Width / cellSize)

        let activeHeight =
            min ((this.Height - offset.Y) / cellSize) (this.Height / cellSize)

        let startX = max (1.0f - this.Camera.Position.X) 1.0f
        let startY = max (1.0f - this.Camera.Position.Y) 1.0f
        let endX = startX + activeWidth
        let endY = startY + activeHeight

        struct (Vector2(startX, startY), Vector2(endX, endY))

    member this.ProcessMouseDrag() =
        if
            not (Mouse.buttonIsPressed MouseButton.Left)
            || Keyboard.keyIsDown KeyboardKey.LeftShift
            || Keyboard.keyIsDown KeyboardKey.LeftShift
        then
            ()
        else
            let mouseDelta = Mouse.getDelta ()
            let cellSizeInverse = 1.0f / (this.CellSize * this.Camera.ZoomFactor)

            let newCameraPosition = (this.Camera.Position + mouseDelta) * cellSizeInverse

            this.Camera.Position <- newCameraPosition

    member this.processMouseScroll() =
        let mousePos = Mouse.position ()

        if
            mousePos.X >= this.Position.X
            && mousePos.X <= this.Position.X + this.Width
            && mousePos.Y >= this.Position.Y
            && mousePos.Y <= this.Position.Y + this.Height
        then
            let mouseScrollAmount = Mouse.getScrollAmount ()
            this.Camera.ZoomIn(mouseScrollAmount.Y * 0.1f)
            this.Camera.ZoomOut(mouseScrollAmount.X * 0.1f)

    member this.ProcessDrawableArea() =
        this.ProcessMouseDrag()
        this.processMouseScroll ()

        let cellSize = this.CellSize * this.Camera.ZoomFactor
        let offset = this.Camera.Position * cellSize

        let rows = Array2D.length1 this.Game.State.Board
        let cols = Array2D.length2 this.Game.State.Board

        let struct (visibleStartPoint, visibleEndPoint) = this.CalculateVisibleRange()

        let startCol = int visibleStartPoint.X
        let startRow = int visibleStartPoint.Y
        let endCol = max (min (int visibleEndPoint.X) (cols - 2)) 1
        let endRow = max (min (int visibleEndPoint.Y) (rows - 2)) 1

        let endBorder = (visibleEndPoint - visibleStartPoint) * cellSize

        for row = startRow to endRow do
            for col = startCol to endCol do
                let trueStartX = float32 col * cellSize + offset.X
                let trueStartY = float32 row * cellSize + offset.Y
                let trueEndX = trueStartX + cellSize
                let trueEndY = trueStartY + cellSize

                let startX = max trueStartX cellSize
                let startY = max trueStartY cellSize
                let endX = min (min (startX + cellSize) endBorder.X) trueEndX
                let endY = min (min (startY + cellSize) endBorder.Y) trueEndY

                if GameArea.IsLeftPressedWithShift startX startY endX endY then
                    GameArea.makeAlive row col this.Game

                if GameArea.IsRightPressedWithShift startX startY endX endY then
                    GameArea.makeDead row col this.Game
