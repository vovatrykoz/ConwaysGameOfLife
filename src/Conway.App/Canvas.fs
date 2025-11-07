namespace Conway.App

open Conway.App.Controls
open Conway.App.Input
open Conway.App.Math
open Conway.Core
open Raylib_cs

type Canvas
    (
        x: float32<px>,
        y: float32<px>,
        width: float32<px>,
        height: float32<px>,
        camera: Camera,
        game: Game,
        cellSize: float32<px>
    ) =

    member val Position = Vec2.create x y with get, set

    member val Width = width with get, set

    member val Height = height with get, set

    member val CellSize = cellSize with get, set

    member val Game = game with get, set

    member val Camera = camera with get, set

    member this.CalculateVisibleRange() : struct (Vec2<cells> * Vec2<cells>) =
        let zoomedCellSize = this.CellSize * this.Camera.ZoomFactor
        let cellsPerPixel = 1.0f<cells> / zoomedCellSize

        let visibleCellsX = this.Width * cellsPerPixel
        let visibleCellsY = this.Height * cellsPerPixel

        let camPosX = this.Camera.Position.X
        let camPosY = this.Camera.Position.Y

        let halfVisibleCellsX = visibleCellsX * 0.5f
        let halfVisibleCellsY = visibleCellsY * 0.5f

        let topLeftCellX = camPosX - halfVisibleCellsX
        let topLeftCellY = camPosY - halfVisibleCellsY

        let startCellX = max (1.0f<cells> + topLeftCellX) 1.0f<cells>
        let startCellY = max (1.0f<cells> + topLeftCellY) 1.0f<cells>

        let visibleRangeX = min (camPosX + halfVisibleCellsX) visibleCellsX - 1.0f<cells>
        let visibleRangeY = min (camPosY + halfVisibleCellsY) visibleCellsY - 1.0f<cells>

        let endCellX = startCellX + visibleRangeX
        let endCellY = startCellY + visibleRangeY

        struct (Vec2.create startCellX startCellY, Vec2.create endCellX endCellY)

    member this.ProcessMouseDrag() =
        let mousePos = Mouse.position () |> Vec2.fromNumericVector

        if
            mousePos.X >= this.Position.X
            && mousePos.X <= this.Position.X + this.Width
            && mousePos.Y >= this.Position.Y
            && mousePos.Y <= this.Position.Y + this.Height
            && Mouse.buttonIsPressed MouseButton.Left
            && not (Keyboard.keyIsDown KeyboardKey.LeftShift)
            && not (Keyboard.keyIsDown KeyboardKey.LeftShift)
        then
            let cellSizeInverse = 1.0f<cells> / (this.CellSize * this.Camera.ZoomFactor)
            let mouseDelta = Mouse.getDelta () |> Vec2<px>.fromNumericVector

            this.Camera.Position <-
                this.Camera.Position
                - Vec2.create (mouseDelta.X * cellSizeInverse) (mouseDelta.Y * cellSizeInverse)

    member this.ProcessMouseScroll() =
        let mousePos = Mouse.position () |> Vec2.fromNumericVector

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

        let startRow = int visibleStartPoint.Y
        let startCol = int visibleStartPoint.X
        let endRow = max (min (int visibleEndPoint.Y) (rows - 2)) 1
        let endCol = max (min (int visibleEndPoint.X) (cols - 2)) 1

        let cellSize = this.CellSize * this.Camera.ZoomFactor
        let scaledCellSize = cellSize / 1.0f<cells>
        let scaledCellSizeReciprocal = 1.0f / scaledCellSize

        let halfWidth = this.Width * 0.5f * scaledCellSizeReciprocal
        let halfHeight = this.Height * 0.5f * scaledCellSizeReciprocal

        let upperLeftCorner =
            Vec2.create (this.Camera.Position.X - halfWidth) (this.Camera.Position.Y - halfHeight)

        let distanceToBorder =
            visibleEndPoint - upperLeftCorner |> Vec2.scaleBy (float32 scaledCellSize)

        for row = startRow to endRow do
            for col = startCol to endCol do
                let baseX =
                    LanguagePrimitives.Float32WithMeasure<cells>(float32 col) - upperLeftCorner.X

                let baseY =
                    LanguagePrimitives.Float32WithMeasure<cells>(float32 row) - upperLeftCorner.Y

                let trueStartX = this.CellSize + (baseX - 1.0f<cells>) * scaledCellSize
                let trueStartY = this.CellSize + (baseY - 1.0f<cells>) * scaledCellSize
                let trueEndX = trueStartX + cellSize
                let trueEndY = trueStartY + cellSize

                let startX = max trueStartX cellSize
                let startY = max trueStartY cellSize

                let endX =
                    min (min (startX + cellSize) (distanceToBorder.X * scaledCellSize)) trueEndX

                let endY =
                    min (min (startY + cellSize) (distanceToBorder.Y * scaledCellSize)) trueEndY

                if GameArea.IsLeftPressedWithShift startX startY endX endY then
                    GameArea.makeAlive row col this.Game

                if GameArea.IsRightPressedWithShift startX startY endX endY then
                    GameArea.makeDead row col this.Game
