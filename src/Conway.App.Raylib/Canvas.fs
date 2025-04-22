namespace Conway.App.Raylib

open Conway.Core
open Raylib_cs

module private CanvasArea =
    let makeAliveCallback row col (game: Game) =
        fun _ ->
            match (ConwayGrid.board game.State)[row, col] with
            | BorderCell -> ()
            | PlayerCell _ -> (ConwayGrid.board game.State)[row, col] <- PlayerCell Cell.living

            // erase the history since the player has altered the board
            game.clearHistory ()

    let makeDeadCallback row col (game: Game) =
        fun _ ->
            match (ConwayGrid.board game.State)[row, col] with
            | BorderCell -> ()
            | PlayerCell _ -> (ConwayGrid.board game.State)[row, col] <- PlayerCell Cell.dead

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
        (callback: unit -> unit)
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
                callback ()
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
        (callback: unit -> unit)
        =
        IsPressedWith x y width height MouseButton.Left offsetX offsetY callback

    let IsRightPressed
        (x: int)
        (y: int)
        (width: int)
        (height: int)
        (offsetX: int)
        (offsetY: int)
        (callback: unit -> unit)
        =
        IsPressedWith x y width height MouseButton.Right offsetX offsetY callback

type Canvas
    (x: int, y: int, width: int, height: int, drawingX: int, drawingY: int, game: Game, cellSize: int, scale: int) =

    member val X = x with get, set

    member val Y = y with get, set

    member val Width = width with get, set

    member val Height = height with get, set

    member val CellSize = cellSize with get, set

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
                let trueX = x + offsetX
                let trueY = y + offsetY

                if
                    trueX < this.X
                    || trueX >= this.X + this.Width
                    || trueY < this.Y
                    || trueY >= this.Y + this.Height
                then
                    ()
                else
                    CanvasArea.IsLeftPressed
                        x
                        y
                        width
                        height
                        offsetX
                        offsetY
                        (CanvasArea.makeAliveCallback row col this.Game)
                    |> ignore

                    CanvasArea.IsRightPressed
                        x
                        y
                        width
                        height
                        offsetX
                        offsetY
                        (CanvasArea.makeDeadCallback row col this.Game)
                    |> ignore

    member this.MoveCameraRight() =
        this.DrawingAreaX <- this.DrawingAreaX - 1

    member this.MoveCameraLeft() =
        this.DrawingAreaX <- this.DrawingAreaX + 1

    member this.MoveCameraUp() =
        this.DrawingAreaY <- this.DrawingAreaY + 1

    member this.MoveCameraDown() =
        this.DrawingAreaY <- this.DrawingAreaY - 1
