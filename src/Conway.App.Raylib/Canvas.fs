namespace Conway.App.Raylib

open Conway.Core
open Raylib_cs
open System.Numerics

module private CanvasArea =
    let makeAlive row col (game: Game) =
        game.State.Board[row, col] <- 1<CellStatus>

        // erase the history since the player has altered the board
        game.ClearHistory()

    let makeDead row col (game: Game) =
        game.State.Board[row, col] <- 0<CellStatus>

        // erase the history since the player has altered the board
        game.ClearHistory()

    let IsPressedWithShift
        (startX: float32)
        (startY: float32)
        (endX: float32)
        (endY: float32)
        (mouseButton: MouseButton)
        =
        if
            (Keyboard.keyIsDown KeyboardKey.LeftShift
             || Keyboard.keyIsDown KeyboardKey.RightShift)
            && Mouse.buttonIsPressed mouseButton
        then
            let mousePos = Mouse.getPosition ()

            if
                mousePos.X >= startX
                && mousePos.X <= endX
                && mousePos.Y >= startY
                && mousePos.Y <= endY
            then
                true
            else
                false
        else
            false

    let IsLeftPressedWithShift (startX: float32) (startY: float32) (endX: float32) (endY: float32) =
        IsPressedWithShift startX startY endX endY MouseButton.Left

    let IsRightPressedWithShift (startX: float32) (startY: float32) (endX: float32) (endY: float32) =
        IsPressedWithShift startX startY endX endY MouseButton.Right

type Canvas
    (
        x: float32,
        y: float32,
        width: float32,
        height: float32,
        drawingX: float32,
        drawingY: float32,
        game: Game,
        cellSize: float32,
        scale: int
    ) =

    let maxCellSizeLimit = 50.0f

    let minCellSizeLimit = 5.0f

    let mutable currentMaxCellSize = maxCellSizeLimit

    let mutable currentMinCellSize = minCellSizeLimit

    member val X = x with get, set

    member val Y = y with get, set

    member val Width = width with get, set

    member val Height = height with get, set

    member val CellSize = cellSize with get, set

    member _.MaxCellSize
        with get () = currentMaxCellSize
        and set value = currentMaxCellSize <- min maxCellSizeLimit (max minCellSizeLimit value)

    member _.MinCellSize
        with get () = currentMinCellSize
        and set value = currentMinCellSize <- min maxCellSizeLimit (max minCellSizeLimit value)

    member val Game = game with get, set

    member val Camera = Camera(-drawingX, -drawingY) with get, set

    member val Scale = scale with get, set

    member this.CalculateVisibleRange() =
        let cellSize = this.CellSize

        let offsetX = this.Camera.X * cellSize
        let offsetY = this.Camera.Y * cellSize

        let activeWidth = min ((this.Width - offsetX) / cellSize) (this.Width / cellSize)
        let activeHeight = min ((this.Height - offsetY) / cellSize) (this.Height / cellSize)

        let startX = max (1.0f - this.Camera.X) 1.0f
        let startY = max (1.0f - this.Camera.Y) 1.0f
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
            let cellSizeInverse = 1.0f / this.CellSize

            this.Camera.X <- this.Camera.X + mouseDelta.X * cellSizeInverse
            this.Camera.Y <- this.Camera.Y + mouseDelta.Y * cellSizeInverse

    member this.processMouseScroll() =
        let mousePos = Mouse.position ()

        if
            mousePos.X >= this.X
            && mousePos.X <= this.X + this.Width
            && mousePos.Y >= this.Y
            && mousePos.Y <= this.Y + this.Height
        then
            let mouseScrollAmount = Mouse.getScrollAmount ()
            this.ZoomIn mouseScrollAmount.Y
            this.ZoomOut mouseScrollAmount.X

    member this.ProcessDrawableArea() =
        this.ProcessMouseDrag()
        this.processMouseScroll ()

        let cellSize = this.CellSize

        let offsetX = this.Camera.X * cellSize
        let offsetY = this.Camera.Y * cellSize

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
                let trueStartX = float32 col * cellSize + offsetX
                let trueStartY = float32 row * cellSize + offsetY
                let trueEndX = trueStartX + cellSize
                let trueEndY = trueStartY + cellSize

                let startX = max trueStartX this.X
                let startY = max trueStartY this.Y
                let endX = min (min (startX + cellSize) endBorderX) trueEndX
                let endY = min (min (startY + cellSize) endBorderY) trueEndY

                if CanvasArea.IsLeftPressedWithShift startX startY endX endY then
                    CanvasArea.makeAlive row col this.Game

                if CanvasArea.IsRightPressedWithShift startX startY endX endY then
                    CanvasArea.makeDead row col this.Game

    member this.ZoomIn(speed: float32) =
        this.CellSize <- min (this.CellSize + speed) this.MaxCellSize

    member this.ZoomOut(speed: float32) =
        this.CellSize <- max (this.CellSize - speed) this.MinCellSize
