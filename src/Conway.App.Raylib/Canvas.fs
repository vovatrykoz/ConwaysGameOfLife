namespace Conway.App.Raylib

open Conway.Core
open Raylib_cs

module private CanvasArea =
    let makeAlive row col (game: Game) =
        game.State.Board[row, col] <- Cell.living

        // erase the history since the player has altered the board
        game.ClearHistory()

    let makeDead row col (game: Game) =
        game.State.Board[row, col] <- Cell.dead

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

    member val DrawingAreaX = -drawingX with get, set

    member val DrawingAreaY = -drawingY with get, set

    member val Scale = scale with get, set

    member this.CalculateVisibleRange() =
        let offsetX = this.DrawingAreaX * this.CellSize
        let offsetY = this.DrawingAreaY * this.CellSize

        let activeWidth =
            min ((this.Width - offsetX) / this.CellSize) (this.Width / this.CellSize)

        let activeHeight =
            min ((this.Height - offsetY) / this.CellSize) (this.Height / this.CellSize)

        let startX = max (1.0f - this.DrawingAreaX) 1.0f
        let startY = max (1.0f - this.DrawingAreaY) 1.0f
        let endX = startX + activeWidth
        let endY = startY + activeHeight

        struct (startX, startY, endX, endY)

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

            this.DrawingAreaX <- this.DrawingAreaX + mouseDelta.X * cellSizeInverse
            this.DrawingAreaY <- this.DrawingAreaY + mouseDelta.Y * cellSizeInverse

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

        let offsetX = this.DrawingAreaX * this.CellSize
        let offsetY = this.DrawingAreaY * this.CellSize

        let rows = Array2D.length1 this.Game.State.Board
        let cols = Array2D.length2 this.Game.State.Board

        let struct (visibleStartX, visibleStartY, visibleEndX, visibleEndY) =
            this.CalculateVisibleRange()

        let startCol = int visibleStartX
        let startRow = int visibleStartY
        let endCol = max (min (int visibleEndX) (cols - 2)) 1
        let endRow = max (min (int visibleEndY) (rows - 2)) 1

        for row = startRow to endRow do
            for col = startCol to endCol do
                let startX = float32 col * this.CellSize + offsetX
                let startY = float32 row * this.CellSize + offsetY
                let endX = startX + this.CellSize
                let endY = startY + this.CellSize

                if CanvasArea.IsLeftPressedWithShift startX startY endX endY then
                    CanvasArea.makeAlive row col this.Game

                if CanvasArea.IsRightPressedWithShift startX startY endX endY then
                    CanvasArea.makeDead row col this.Game

    member this.MoveCameraRight(speed: float32) =
        this.DrawingAreaX <- this.DrawingAreaX - speed

    member this.MoveCameraLeft(speed: float32) =
        this.DrawingAreaX <- this.DrawingAreaX + speed

    member this.MoveCameraUp(speed: float32) =
        this.DrawingAreaY <- this.DrawingAreaY + speed

    member this.MoveCameraDown(speed: float32) =
        this.DrawingAreaY <- this.DrawingAreaY - speed

    member this.ZoomIn(speed: float32) =
        this.CellSize <- min (this.CellSize + speed) this.MaxCellSize

    member this.ZoomOut(speed: float32) =
        this.CellSize <- max (this.CellSize - speed) this.MinCellSize
