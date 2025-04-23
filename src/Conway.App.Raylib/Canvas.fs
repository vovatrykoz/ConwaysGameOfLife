namespace Conway.App.Raylib

open Conway.Core
open Raylib_cs

module private CanvasArea =
    let makeAlive row col (game: Game) =
        (ConwayGrid.board game.State)[row, col] <- Cell.living

        // erase the history since the player has altered the board
        game.clearHistory ()

    let makeDead row col (game: Game) =
        (ConwayGrid.board game.State)[row, col] <- Cell.dead

        // erase the history since the player has altered the board
        game.clearHistory ()

    let IsPressedWith
        (x: int)
        (y: int)
        (width: int)
        (height: int)
        (mouseButton: MouseButton)
        (offsetX: int)
        (offsetY: int)
        =
        if Mouse.readButtonPress mouseButton then
            let trueX = x + offsetX
            let trueY = y + offsetY

            let minX = trueX
            let maxX = trueX + width
            let minY = trueY
            let maxY = trueY + height

            let mousePos = Mouse.getPosition ()

            if
                mousePos.X >= float32 minX
                && mousePos.X <= float32 maxX
                && mousePos.Y >= float32 minY
                && mousePos.Y <= float32 maxY
            then
                true
            else
                false
        else
            false

    let IsLeftPressed
        (x: int)
        (y: int)
        (width: int)
        (height: int)
        (offsetX: int)
        (offsetY: int)
        =
        IsPressedWith x y width height MouseButton.Left offsetX offsetY

    let IsRightPressed
        (x: int)
        (y: int)
        (width: int)
        (height: int)
        (offsetX: int)
        (offsetY: int)
        =
        IsPressedWith x y width height MouseButton.Right offsetX offsetY

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
        let struct (startX, startY, endX, endY) = this.CalculateVisibleRange()

        for row in startY..endY do
            for col in startX..endX do
                let x = col * this.CellSize
                let y = row * this.CellSize
                let width = this.CellSize
                let height = this.CellSize

                if CanvasArea.IsLeftPressed x y width height offsetX offsetY then 
                    CanvasArea.makeAlive row col this.Game

                if CanvasArea.IsRightPressed x y width height offsetX offsetY then
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
