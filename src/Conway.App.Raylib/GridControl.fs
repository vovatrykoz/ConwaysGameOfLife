namespace Conway.App.Raylib

open Raylib_cs

type GridControl
    (x: int, y: int, width: int, height: int, onLeftClick: option<unit -> unit>, onRightClick: option<unit -> unit>) =
    member val X = x with get, set

    member val Y = y with get, set

    member val Width = width with get, set

    member val Height = height with get, set

    member val OnLeftClick = onLeftClick with get, set

    member val OnRightClick = onRightClick with get, set

    static member IsPressedWith(gridControl: GridControl, mouseButton: MouseButton, callback: option<unit -> unit>) =
        if Mouse.readButtonPress mouseButton then
            let mousePos = Mouse.getPosition ()
            let minX = gridControl.X
            let maxX = gridControl.X + gridControl.Width
            let minY = gridControl.Y
            let maxY = gridControl.Y + gridControl.Height

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

    static member IsLeftPressed(gridControl: GridControl) =
        GridControl.IsPressedWith(gridControl, MouseButton.Left, gridControl.OnLeftClick)

    static member IsRightPressed(gridControl: GridControl) =
        GridControl.IsPressedWith(gridControl, MouseButton.Right, gridControl.OnRightClick)

    static member create = new GridControl(0, 0, 0, 0, None, None)

    static member position x y (gridControl: GridControl) =
        gridControl.X <- x
        gridControl.Y <- y
        gridControl

    static member width width (gridControl: GridControl) =
        gridControl.Width <- width
        gridControl

    static member height height (gridControl: GridControl) =
        gridControl.Height <- height
        gridControl

    static member onLeftClickCallback callback (gridControl: GridControl) =
        gridControl.OnLeftClick <- Some callback
        gridControl

    static member onRightClickCallback callback (gridControl: GridControl) =
        gridControl.OnRightClick <- Some callback
        gridControl
