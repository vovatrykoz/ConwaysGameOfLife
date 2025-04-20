namespace Conway.App.Raylib

type Canvas(x: int, y: int, canvasSize: int, drawingAreaSize: int, scale: int) =
    member val X = x with get, set

    member val Y = y with get, set

    member val Size = canvasSize with get, set

    member val DrawingAreaX = x with get, set

    member val DrawingAreaY = y with get, set

    member val DrawingAreaSize = drawingAreaSize with get, set

    member val Scale = scale with get, set

    member val Controls = seq<CanvasControl> Seq.empty with get, set

    member this.AddControl(canvasControl: CanvasControl) =
        this.Controls <- seq { canvasControl } |> Seq.append this.Controls

    member this.AddControls(canvasControls: seq<CanvasControl>) =
        this.Controls <- canvasControls |> Seq.append this.Controls

    member this.ProcessControls() =
        this.Controls
        |> Seq.iter (fun canvasControl ->
            CanvasControl.IsLeftPressed canvasControl |> ignore
            CanvasControl.IsRightPressed canvasControl |> ignore)
