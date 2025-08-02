namespace Conway.App.Controls

open System
open System.Collections.Generic
open Conway.App.Input
open Raylib_cs
open System.Collections.ObjectModel

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

type FilePicker(x: int, y: int, fileEntryHeight: int, fileEntryWidth: int, files: seq<FileData>) =
    let mutable _currentSelection: FileData option = None

    new(x: int, y: int, fileEntryHeight: int, fileEntryWidth: int) =
        new FilePicker(x, y, fileEntryHeight, fileEntryWidth, Seq.empty)

    member val X = x with get, set

    member val Y = y with get, set

    member val FileEntryWidth = fileEntryWidth with get, set

    member val FileEntryHeight = fileEntryHeight with get, set

    member val Files = new ObservableCollection<FileData>(files) with get

    member _.CurrentSelection
        with get () = _currentSelection
        and private set value = _currentSelection <- value

    member this.SelectAt index =
        if index < 0 || index >= this.Files.Count then
            this.CurrentSelection <- None
        else
            this.CurrentSelection <- Some(this.Files.[index])

    member this.ClearSelection() = this.CurrentSelection <- None

    member this.ProcessInput() =
        if Mouse.buttonClicked MouseButton.Left then
            let mousePos = Mouse.getPosition ()

            this.Files
            |> Seq.iteri (fun index _ ->
                let minX = this.X
                let maxX = this.X + this.FileEntryWidth
                let minY = this.Y + this.FileEntryHeight * index
                let maxY = minY + this.FileEntryHeight

                if
                    mousePos.X >= float32 minX
                    && mousePos.X <= float32 maxX
                    && mousePos.Y >= float32 minY
                    && mousePos.Y <= float32 maxY
                then
                    this.SelectAt index)
