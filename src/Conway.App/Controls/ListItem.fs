namespace Conway.App.Controls

open Raylib_cs
open Conway.App.Input

type ListItem(x: int, y: int, size: int, isSelected: bool) =
    new(x: int, y: int, size: int) = new ListItem(x, y, size, false)

    member val X = x with get, set

    member val Y = y with get, set

    member val Size = size with get, set

    member val IsSelected = isSelected with get, set

    static member isClicked(listItem: ListItem) =
        if Mouse.buttonClicked MouseButton.Left then
            let mousePos = Mouse.getPosition ()
            let minX = listItem.X
            let maxX = listItem.X + listItem.Size
            let minY = listItem.Y
            let maxY = listItem.Y + listItem.Size

            if
                mousePos.X >= float32 minX
                && mousePos.X <= float32 maxX
                && mousePos.Y >= float32 minY
                && mousePos.Y <= float32 maxY
            then
                listItem.IsSelected <- true
                listItem.IsSelected
            else
                listItem.IsSelected <- false
                listItem.IsSelected
        else
            listItem.IsSelected <- false
            listItem.IsSelected
