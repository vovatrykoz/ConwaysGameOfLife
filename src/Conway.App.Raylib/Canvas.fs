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
        let visibleCellSize = this.CellSize * this.Camera.ZoomFactor
        let visibleCellSizeReciprocal = 1.0f / visibleCellSize
        let horizontalCellCount = this.Width * visibleCellSizeReciprocal
        let verticalCellCount = this.Height * visibleCellSizeReciprocal

        let halfWidth = this.Width * 0.5f * visibleCellSizeReciprocal
        let halfHeight = this.Height * 0.5f * visibleCellSizeReciprocal

        let upperLeftCornerX = this.Camera.Position.X - halfWidth
        let upperLeftCornerY = this.Camera.Position.Y - halfHeight

        let visibleWidth = min (horizontalCellCount + upperLeftCornerX) horizontalCellCount
        let visibleHeigh = min (verticalCellCount + upperLeftCornerY) verticalCellCount

        let startX = max (1.0f + upperLeftCornerX) 1.0f
        let startY = max (1.0f + upperLeftCornerY) 1.0f
        let endX = startX + visibleWidth
        let endY = startY + visibleHeigh

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

            this.Camera.Position <- this.Camera.Position - mouseDelta * cellSizeInverse

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

        let rows = Array2D.length1 this.Game.State.Board
        let cols = Array2D.length2 this.Game.State.Board

        let struct (visibleStartPoint, visibleEndPoint) = this.CalculateVisibleRange()

        let startCol = int visibleStartPoint.X
        let startRow = int visibleStartPoint.Y
        let endCol = max (min (int visibleEndPoint.X) (cols - 2)) 1
        let endRow = max (min (int visibleEndPoint.Y) (rows - 2)) 1

        let visibleCellSize = this.CellSize * this.Camera.ZoomFactor
        let visibleCellSizeReciprocal = 1.0f / visibleCellSize

        let halfWidth = this.Width * 0.5f * visibleCellSizeReciprocal
        let halfHeight = this.Height * 0.5f * visibleCellSizeReciprocal

        let upperLeftCornerX = this.Camera.Position.X - halfWidth
        let upperLeftCornerY = this.Camera.Position.Y - halfHeight

        let endBorderX = (visibleEndPoint.X - visibleStartPoint.X) * visibleCellSize
        let endBorderY = (visibleEndPoint.Y - visibleStartPoint.Y) * visibleCellSize

        for row = startRow to endRow do
            for col = startCol to endCol do
                let trueStartX = float32 col * visibleCellSize - upperLeftCornerX
                let trueStartY = float32 row * visibleCellSize - upperLeftCornerY
                let trueEndX = trueStartX + visibleCellSize
                let trueEndY = trueStartY + visibleCellSize

                let startX = max trueStartX visibleCellSize
                let startY = max trueStartY visibleCellSize
                let endX = min (min (startX + visibleCellSize) endBorderX) trueEndX
                let endY = min (min (startY + visibleCellSize) endBorderY) trueEndY

                if GameArea.IsLeftPressedWithShift startX startY endX endY then
                    GameArea.makeAlive row col this.Game

                if GameArea.IsRightPressedWithShift startX startY endX endY then
                    GameArea.makeDead row col this.Game
