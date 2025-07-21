namespace Conway.App

open Conway.App.Input
open Conway.App.Controls
open Conway.Core
open Raylib_cs
open System.Numerics

type Canvas(x: float32, y: float32, width: float32, height: float32, camera: Camera, game: Game, cellSize: float32) =

    member val Position = Vector2(x, y) with get, set

    member val Width = width with get, set

    member val Height = height with get, set

    member val CellSize = cellSize with get, set

    member val Game = game with get, set

    member val Camera = camera with get, set

    member this.CalculateVisibleRange() =
        let visibleCellSize = this.CellSize * this.Camera.ZoomFactor
        let visibleCellSizeReciprocal = 1.0f / visibleCellSize
        let horizontalCellCount = this.Width * visibleCellSizeReciprocal
        let verticalCellCount = this.Height * visibleCellSizeReciprocal

        let camX = this.Camera.Position.X
        let camY = this.Camera.Position.Y

        let halfWidth = horizontalCellCount * 0.5f
        let halfHeight = verticalCellCount * 0.5f

        let upperLeftCornerX = camX - halfWidth
        let upperLeftCornerY = camY - halfHeight

        let startX = max (1.0f + upperLeftCornerX) 1.0f
        let startY = max (1.0f + upperLeftCornerY) 1.0f

        let visibleWidth = min (halfWidth + camX) horizontalCellCount - 1.0f
        let visibleHeigh = min (halfHeight + camY) verticalCellCount - 1.0f

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

    member this.ProcessMouseScroll() =
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
        this.ProcessMouseScroll()

        let rows = Array2D.length1 this.Game.CurrentState.Board
        let cols = Array2D.length2 this.Game.CurrentState.Board

        let struct (visibleStartPoint, visibleEndPoint) = this.CalculateVisibleRange()

        let startCol = int visibleStartPoint.X
        let startRow = int visibleStartPoint.Y
        let endCol = max (min (int visibleEndPoint.X) (cols - 2)) 1
        let endRow = max (min (int visibleEndPoint.Y) (rows - 2)) 1

        let scaledCellSize = this.CellSize * this.Camera.ZoomFactor
        let scaledCellSizeReciprocal = 1.0f / scaledCellSize

        let halfWidth = this.Width * 0.5f * scaledCellSizeReciprocal
        let halfHeight = this.Height * 0.5f * scaledCellSizeReciprocal

        let upperLeftCorner =
            Vector2(this.Camera.Position.X - halfWidth, this.Camera.Position.Y - halfHeight)

        let distanceToBorder = (visibleEndPoint - upperLeftCorner) * scaledCellSize

        for row = startRow to endRow do
            for col = startCol to endCol do
                let baseX = float32 col - upperLeftCorner.X
                let baseY = float32 row - upperLeftCorner.Y

                let trueStartX = this.CellSize + (baseX - 1.0f) * scaledCellSize
                let trueStartY = this.CellSize + (baseY - 1.0f) * scaledCellSize
                let trueEndX = trueStartX + scaledCellSize
                let trueEndY = trueStartY + scaledCellSize

                let startX = max trueStartX scaledCellSize
                let startY = max trueStartY scaledCellSize

                let endX = min (min (startX + scaledCellSize) distanceToBorder.X) trueEndX
                let endY = min (min (startY + scaledCellSize) distanceToBorder.Y) trueEndY

                if GameArea.IsLeftPressedWithShift startX startY endX endY then
                    GameArea.makeAlive row col this.Game

                if GameArea.IsRightPressedWithShift startX startY endX endY then
                    GameArea.makeDead row col this.Game
