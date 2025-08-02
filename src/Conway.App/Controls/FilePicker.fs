namespace Conway.App.Controls

open System
open System.Collections.Generic

[<NoComparison>]
type FileData = {
    Name: string
    Path: string
    Date: DateTime
} with

    static member createRecord name path date = {
        Name = name
        Path = path
        Date = date
    }

type FilePicker(files: seq<FileData>) =
    let mutable _currentSelection: FileData option = None

    new() = new FilePicker(Seq.empty)

    member val Files = new List<FileData>(files) with get

    member _.CurrentSelection
        with get () = _currentSelection
        and private set value = _currentSelection <- value

    member this.SelectAt index =
        if index < 0 || index >= this.Files.Count then
            this.CurrentSelection <- None
        else
            this.CurrentSelection <- Some(this.Files.[index])

    member this.ClearSelection() = this.CurrentSelection <- None
