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

    member val Camera = Camera(drawingX, drawingY) with get, set

    member this.CalculateVisibleRange() =
        let cellSize = this.CellSize * this.Camera.ZoomFactor
        let cellSizeReciprocal = 1.0f / cellSize
        let widthDivByCellSize = this.Width * cellSizeReciprocal
        let heightDivByCellSize = this.Height * cellSizeReciprocal

        let camX = this.Camera.Position.X
        let camY = this.Camera.Position.Y

        let activeWidth = min (widthDivByCellSize + camX) widthDivByCellSize
        let activeHeight = min (heightDivByCellSize + camY) heightDivByCellSize

        let startX = max (1.0f + camX) 1.0f
        let startY = max (1.0f + camY) 1.0f
        let endX = startX + activeWidth
        let endY = startY + activeHeight

        struct (Vector2(startX, startY), Vector2(endX, endY))

    member this.ProcessMouseDrag() =
        let mousePos = Mouse.position ()

        if
            mousePos.X >= this.Position.X
            && mousePos.X <= this.Position.X + this.Width
            && mousePos.Y >= this.Position.Y
            && mousePos.Y <= this.Position.Y + this.Height
            && Mouse.buttonIsPressed MouseButton.Left
            && not (Keyboard.keyIsDown KeyboardKey.LeftShift)
            && not (Keyboard.keyIsDown KeyboardKey.LeftShift)
        then
            let mouseDelta = Mouse.getDelta ()
            let cellSizeInverse = 1.0f / (this.CellSize * this.Camera.ZoomFactor)

            this.Camera.Position <-
                Vector2(
                    this.Camera.Position.X - mouseDelta.X * cellSizeInverse,
                    this.Camera.Position.Y - mouseDelta.Y * cellSizeInverse
                )

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

        let offsetX = this.Camera.Position.X * cellSize
        let offsetY = this.Camera.Position.Y * cellSize

        let rows = Array2D.length1 this.Game.State.Board
        let cols = Array2D.length2 this.Game.State.Board

        let struct (visibleStartPoint, visibleEndPoint) = this.CalculateVisibleRange()

        let startCol = int visibleStartPoint.X
        let startRow = int visibleStartPoint.Y
        let endCol = max (min (int visibleEndPoint.X) (cols - 2)) 1
        let endRow = max (min (int visibleEndPoint.Y) (rows - 2)) 1

        let endBorderX = (visibleEndPoint.X - visibleStartPoint.X) * cellSize
        let endBorderY = (visibleEndPoint.Y - visibleStartPoint.Y) * cellSize

        for row = startRow to endRow do
            for col = startCol to endCol do
                let trueStartX = float32 col * cellSize - offsetX
                let trueStartY = float32 row * cellSize - offsetY
                let trueEndX = trueStartX + cellSize
                let trueEndY = trueStartY + cellSize

                let startX = max trueStartX cellSize
                let startY = max trueStartY cellSize
                let endX = min (min (startX + cellSize) endBorderX) trueEndX
                let endY = min (min (startY + cellSize) endBorderY) trueEndY

                if GameArea.IsLeftPressedWithShift startX startY endX endY then
                    GameArea.makeAlive row col this.Game

                if GameArea.IsRightPressedWithShift startX startY endX endY then
                    GameArea.makeDead row col this.Game
