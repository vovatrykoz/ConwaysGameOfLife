namespace Conway.App.Controls

open System
open System.Collections.Generic
open Conway.App.Input
open Raylib_cs
open System.Collections.ObjectModel

[<Struct; NoComparison>]
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
    let mutable _currentSelection: int option = None

    new(x: int, y: int, fileEntryHeight: int, fileEntryWidth: int) =
        new FilePicker(x, y, fileEntryHeight, fileEntryWidth, Seq.empty)

    member val X = x with get, set

    member val Y = y with get, set

    member val FileEntryWidth = fileEntryWidth with get, set

    member val FileEntryHeight = fileEntryHeight with get, set

    member val Files = new ObservableCollection<FileData>(files) with get

    member this.CurrentSelection
        with get () =
            match _currentSelection with
            | None -> None
            | Some index -> Some this.Files.[index]
        and private set value =
            match value with
            | None -> _currentSelection <- None
            | Some fileData ->
                let index = this.Files.IndexOf fileData

                match index with
                | -1 -> _currentSelection <- None
                | _ -> _currentSelection <- Some index

    member this.SelectAt index =
        if index < 0 || index >= this.Files.Count then
            _currentSelection <- None
        else
            _currentSelection <- Some index

    member this.ClearSelection() = this.CurrentSelection <- None

    member this.ProcessMouseInput() =
        if Mouse.buttonClicked MouseButton.Left then
            let mousePos = Mouse.getPosition ()

            _currentSelection <- None

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

    member this.ProcessKeyboardInput() =
        match _currentSelection with
        | None -> ()
        | Some selectedIndex ->
            if Keyboard.keyHasBeenPressedOnce KeyboardKey.Down then
                _currentSelection <- Some(min (selectedIndex + 1) (this.Files.Count - 1))
            else if Keyboard.keyHasBeenPressedOnce KeyboardKey.Up then
                _currentSelection <- Some(max (selectedIndex - 1) 0)

    member this.ProcessInput() =
        this.ProcessMouseInput()
        this.ProcessKeyboardInput()
