namespace Conway.App.Raylib

open Conway.Core

module private ControlsInitializer =
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

    let initFrom (game: Game) width height =
        let board = game.State |> ConwayGrid.board
        let rows = board |> Array2D.length1
        let cols = board |> Array2D.length2
        let total = rows * cols

        let controls: array<CanvasControl> =
            Array.init total (fun i ->
                let row = i / rows
                let col = i % rows

                CanvasControl.create
                |> CanvasControl.position (col * width) (row * height)
                |> CanvasControl.width width
                |> CanvasControl.height height
                |> CanvasControl.onLeftClickCallback (makeAliveCallback row col game)
                |> CanvasControl.onRightClickCallback (makeDeadCallback row col game))

        controls

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

    member val Controls = ControlsInitializer.initFrom game baseCellSize baseCellSize with get, set

    member this.ProcessControls() =
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

        for row in startY..endY do
            for col in startX..endX do
                let canvasControl = this.Controls[row * this.Columns + col]
                let trueX = canvasControl.X + offsetX
                let trueY = canvasControl.Y + offsetY

                if
                    trueX < this.X
                    || trueX >= this.X + this.Width
                    || trueY < this.Y
                    || trueY >= this.Y + this.Height
                then
                    ()
                else
                    CanvasControl.IsLeftPressed(canvasControl, offsetX, offsetY) |> ignore

                    CanvasControl.IsRightPressed(canvasControl, offsetX, offsetY) |> ignore

    member this.MoveCameraRight() =
        this.DrawingAreaX <- this.DrawingAreaX - 1

    member this.MoveCameraLeft() =
        this.DrawingAreaX <- this.DrawingAreaX + 1

    member this.MoveCameraUp() =
        this.DrawingAreaY <- this.DrawingAreaY + 1

    member this.MoveCameraDown() =
        this.DrawingAreaY <- this.DrawingAreaY - 1
