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
    (x: int, y: int, width: int, height: int, drawingX: int, drawingY: int, game: Game, baseCellSize: int, scale: int) =

    member val X = x with get, set

    member val Y = y with get, set

    member val Width = width with get, set

    member val Height = height with get, set

    member val BaseCellSize = baseCellSize with get, set

    member val Rows = ConwayGrid.board game.State |> Array2D.length1 with get

    member val Columns = ConwayGrid.board game.State |> Array2D.length2 with get

    member val DrawingAreaX = drawingX with get, set

    member val DrawingAreaY = drawingY with get, set

    member val Scale = scale with get, set

    member this.CalculateVisibleRange() =
        let offsetX = this.DrawingAreaX * this.BaseCellSize
        let offsetY = this.DrawingAreaY * this.BaseCellSize

        let activeWidth =
            min ((this.Width - offsetX) / this.BaseCellSize) (this.Width / this.BaseCellSize)

        let activeHeight =
            min ((this.Height - offsetY) / this.BaseCellSize) (this.Height / this.BaseCellSize)

        let startX = max (1 - this.DrawingAreaX) 1
        let startY = max (1 - this.DrawingAreaY) 1
        let endX = startX + activeWidth
        let endY = startY + activeHeight

        startX, startY, endX, endY

    member this.ProcessControls() =
        let offsetX = this.DrawingAreaX * this.BaseCellSize
        let offsetY = this.DrawingAreaY * this.BaseCellSize
        let startX, startY, endX, endY = this.CalculateVisibleRange()

        for row in startY..endY do
            for col in startX..endX do
                let x = col * this.BaseCellSize
                let y = row * this.BaseCellSize
                let width = this.BaseCellSize
                let height = this.BaseCellSize
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
                        (CanvasArea.makeAliveCallback row col game)
                    |> ignore

                    CanvasArea.IsRightPressed
                        x
                        y
                        width
                        height
                        offsetX
                        offsetY
                        (CanvasArea.makeDeadCallback row col game)
                    |> ignore

    member this.MoveCameraRight() =
        this.DrawingAreaX <- this.DrawingAreaX - 1

    member this.MoveCameraLeft() =
        this.DrawingAreaX <- this.DrawingAreaX + 1

    member this.MoveCameraUp() =
        this.DrawingAreaY <- this.DrawingAreaY + 1

    member this.MoveCameraDown() =
        this.DrawingAreaY <- this.DrawingAreaY - 1
