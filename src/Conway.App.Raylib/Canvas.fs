namespace Conway.App.Raylib

open Conway.Core

module private ControlsInitializer =
    let initFrom (game: Game) width height =
        let mutable controls = seq<CanvasControl> Seq.empty

        game.State.Board
        |> Array2D.iteri (fun row col cellType ->
            match cellType with
            | BorderCell -> ()
            | PlayerCell _ ->
                let makeAliveCallback =
                    fun _ ->
                        match game.State.Board[row, col] with
                        | BorderCell -> ()
                        | PlayerCell _ -> game.State.Board[row, col] <- (PlayerCell Cell.living)

                        // erase the history since the player has altered the board
                        game.clearHistory ()

                let makeDeadCallback =
                    fun _ ->
                        match game.State.Board[row, col] with
                        | BorderCell -> ()
                        | PlayerCell _ -> game.State.Board[row, col] <- (PlayerCell Cell.dead)

                        // erase the history since the player has altered the board
                        game.clearHistory ()

                controls <-
                    seq {
                        CanvasControl.create
                        |> CanvasControl.position (col * width) (row * height)
                        |> CanvasControl.width width
                        |> CanvasControl.height height
                        |> CanvasControl.onLeftClickCallback makeAliveCallback
                        |> CanvasControl.onRightClickCallback makeDeadCallback
                    }
                    |> Seq.append controls)

        controls

type Canvas(x: int, y: int, game: Game, baseCellSize: int, scale: int) =

    member val X = x with get, set

    member val Y = y with get, set

    member val BaseCellSize = baseCellSize with get, set

    member val Rows = game.State.Board |> Array2D.length1 with get

    member val Columns = game.State.Board |> Array2D.length2 with get

    member val DrawingAreaX = x with get, set

    member val DrawingAreaY = y with get, set

    member val Scale = scale with get, set

    member val Controls = ControlsInitializer.initFrom game baseCellSize baseCellSize with get, set

    member this.ProcessControls() =
        this.Controls
        |> Seq.iter (fun canvasControl ->
            CanvasControl.IsLeftPressed canvasControl |> ignore
            CanvasControl.IsRightPressed canvasControl |> ignore)

    member this.MoveCameraRight() =
        this.DrawingAreaX <- this.DrawingAreaX - 1

    member this.MoveCameraLeft() =
        this.DrawingAreaX <- this.DrawingAreaX + 1

    member this.MoveCameraUp() =
        this.DrawingAreaY <- this.DrawingAreaY + 1

    member this.MoveCameraDown() =
        this.DrawingAreaY <- this.DrawingAreaY - 1
