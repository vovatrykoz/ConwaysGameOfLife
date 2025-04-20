namespace Conway.App.Raylib

open Raylib_cs

type CanvasControl
    (
        x: int,
        y: int,
        width: int,
        height: int,
        onLeftClick: option<unit -> unit>,
        onRightClick: option<unit -> unit>,
        isActive: bool
    ) =
    member val X = x with get, set

    member val Y = y with get, set

    member val Width = width with get, set

    member val Height = height with get, set

    member val OnLeftClick = onLeftClick with get, set

    member val OnRightClick = onRightClick with get, set

    member val isActive = isActive with get, set

    static member IsPressedWith
        (canvasControl: CanvasControl, mouseButton: MouseButton, callback: option<unit -> unit>)
        =
        if Mouse.readButtonPress mouseButton then
            let mousePos = Mouse.getPosition ()
            let minX = canvasControl.X
            let maxX = canvasControl.X + canvasControl.Width
            let minY = canvasControl.Y
            let maxY = canvasControl.Y + canvasControl.Height

            if
                mousePos.X >= float32 minX
                && mousePos.X <= float32 maxX
                && mousePos.Y >= float32 minY
                && mousePos.Y <= float32 maxY
            then
                match callback with
                | None -> ()
                | Some func -> func ()

                true
            else
                false
        else
            false

    static member IsLeftPressed(gridControl: CanvasControl) =
        CanvasControl.IsPressedWith(gridControl, MouseButton.Left, gridControl.OnLeftClick)

    static member IsRightPressed(gridControl: CanvasControl) =
        CanvasControl.IsPressedWith(gridControl, MouseButton.Right, gridControl.OnRightClick)

    static member create = new CanvasControl(0, 0, 0, 0, None, None, true)

    static member position x y (gridControl: CanvasControl) =
        gridControl.X <- x
        gridControl.Y <- y
        gridControl

    static member width width (gridControl: CanvasControl) =
        gridControl.Width <- width
        gridControl

    static member height height (gridControl: CanvasControl) =
        gridControl.Height <- height
        gridControl

    static member onLeftClickCallback callback (gridControl: CanvasControl) =
        gridControl.OnLeftClick <- Some callback
        gridControl

    static member onRightClickCallback callback (gridControl: CanvasControl) =
        gridControl.OnRightClick <- Some callback
        gridControl
