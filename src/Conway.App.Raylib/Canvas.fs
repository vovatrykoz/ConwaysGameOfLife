namespace Conway.App.Raylib

type Canvas(x: int, y: int, canvasSize: int, drawingAreaSize: int, scale: int) =
    member val X = x with get, set

    member val Y = y with get, set

    member val Size = canvasSize with get, set

    member val DrawingAreaSize = drawingAreaSize with get, set

    member val Scale = scale with get, set

    member val GridControls = seq<GridControl> Seq.empty with get, set

    member this.ProcessControls() =
        this.GridControls
        |> Seq.iter (fun gridControl ->
            GridControl.IsLeftPressed gridControl |> ignore
            GridControl.IsRightPressed gridControl |> ignore)
