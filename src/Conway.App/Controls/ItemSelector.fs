namespace Conway.App.Controls

open System.Collections.Generic

type ItemSelector<'T when 'T: equality>(items: seq<'T>) =
    let mutable _selectedIndex: int option = None

    member val Items = new List<'T>(items) with get

    member this.SelectedItem =
        match _selectedIndex with
        | None -> None
        | Some index -> Some this.Items[index]

    member this.SelectItem(index: int) =
        if index >= 0 && index < this.Items.Count then
            _selectedIndex <- Some index
        else
            _selectedIndex <- None

        this.SelectedItem

    member this.SelectItem(item: 'T) =
        let itemIndex = this.Items.FindIndex(fun listItem -> listItem.Equals item)

        match itemIndex with
        | -1 -> _selectedIndex <- None
        | x -> _selectedIndex <- Some x

    member this.DeselectItem(item: 'T) =
        match _selectedIndex with
        | Some i when this.Items[i] = item -> _selectedIndex <- None
        | _ -> ()

    member _.ClearSelection() = _selectedIndex <- None
