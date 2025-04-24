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

    let IsPressedWith (startX: int) (startY: int) (endX: int) (endY: int) (mouseButton: MouseButton) =
        if Mouse.readButtonPress mouseButton then
            let mousePos = Mouse.getPosition ()

            if
                mousePos.X >= float32 startX
                && mousePos.X <= float32 endX
                && mousePos.Y >= float32 startY
                && mousePos.Y <= float32 endY
            then
                true
            else
                false
        else
            false

    let IsLeftPressed (startX: int) (startY: int) (endX: int) (endY: int) =
        IsPressedWith startX startY endX endY MouseButton.Left

    let IsRightPressed (startX: int) (startY: int) (endX: int) (endY: int) =
        IsPressedWith startX startY endX endY MouseButton.Right

type Canvas
    (x: int, y: int, width: int, height: int, drawingX: int, drawingY: int, game: Game, cellSize: int, scale: int) =

    let maxCellSizeLimit = 50

    let minCellSizeLimit = 5

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

        let startX = max (1 - this.DrawingAreaX) 1
        let startY = max (1 - this.DrawingAreaY) 1
        let endX = startX + activeWidth
        let endY = startY + activeHeight

        struct (startX, startY, endX, endY)

    member this.ProcessDrawableArea() =
        let offsetX = this.DrawingAreaX * this.CellSize
        let offsetY = this.DrawingAreaY * this.CellSize

        let rows = Array2D.length1 this.Game.State.Board
        let cols = Array2D.length2 this.Game.State.Board

        let struct (visibleStartX, visibleStartY, visibleEndX, visibleEndY) =
            this.CalculateVisibleRange()

        let adjustedEndX = max (min visibleEndX (cols - 2)) 1
        let adjustedEndY = max (min visibleEndY (rows - 2)) 1

        for row = visibleStartX to adjustedEndY do
            for col = visibleStartY to adjustedEndX do
                let startX = col * this.CellSize + offsetX
                let startY = row * this.CellSize + offsetY
                let endX = startX + this.CellSize
                let endY = startY + this.CellSize

                if CanvasArea.IsLeftPressed startX startY endX endY then
                    CanvasArea.makeAlive row col this.Game

                if CanvasArea.IsRightPressed startX startY endX endY then
                    CanvasArea.makeDead row col this.Game

    member this.MoveCameraRight(speed: int) =
        this.DrawingAreaX <- this.DrawingAreaX - speed

    member this.MoveCameraLeft(speed: int) =
        this.DrawingAreaX <- this.DrawingAreaX + speed

    member this.MoveCameraUp(speed: int) =
        this.DrawingAreaY <- this.DrawingAreaY + speed

    member this.MoveCameraDown(speed: int) =
        this.DrawingAreaY <- this.DrawingAreaY - speed

    member this.ZoomIn(speed: int) =
        this.CellSize <- min (this.CellSize + speed) this.MaxCellSize

    member this.ZoomOut(speed: int) =
        this.CellSize <- max (this.CellSize - speed) this.MinCellSize
