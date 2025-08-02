namespace Conway.App.Controls

open System
open System.Collections.Generic
open Conway.App.Input
open Raylib_cs
open System.Collections.ObjectModel
open System.Numerics

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

type FilePicker
    (
        x: float32,
        y: float32,
        width: float32,
        height: float32,
        fileEntryWidth: float32,
        fileEntryHeight: float32,
        files: seq<FileData>
    ) =
    let mutable _currentSelection: int option = None

    let mutable _cancelled = false

    let mutable _confirmed = false

    let _confirmButton =
        new Button(
            int x,
            int (y + fileEntryHeight * 11.0f),
            100,
            40,
            "Confirm",
            false,
            true,
            Some(fun _ -> _confirmed <- true),
            None,
            None,
            Some KeyboardKey.Enter
        )

    let _cancelButton =
        new Button(
            int x + 150,
            int (y + fileEntryHeight * 11.0f),
            100,
            40,
            "Cancel",
            true,
            true,
            Some(fun _ -> _cancelled <- true),
            None,
            None,
            Some KeyboardKey.Escape
        )

    new(x: float32, y: float32, width: float32, height: float32, fileEntryWidth: float32, fileEntryHeight: float32) =
        new FilePicker(x, y, width, height, fileEntryWidth, fileEntryHeight, Seq.empty)

    member val X = x with get, set

    member val Y = y with get, set

    member val Width = width with get, set

    member val Height = height with get, set

    member val FileEntryWidth = fileEntryWidth with get, set

    member val FileEntryHeight = fileEntryHeight with get, set

    member val Files = new ObservableCollection<FileData>(files) with get

    member val Camera = new Camera(x, y) with get

    member val private ActivatedButton: option<Button> = None with get, set

    member _.Cancelled
        with get () = _cancelled
        and private set value = _cancelled <- value

    member _.Confirmed
        with get () = _confirmed
        and private set value = _confirmed <- value

    member _.ConfirmButton = _confirmButton

    member _.CancelButton = _cancelButton

    member this.CurrentSelection
        with get () =
            match _currentSelection with
            | None -> None
            | Some index -> Some this.Files.[index]
        and private set value =
            match value with
            | None ->
                _currentSelection <- None
                _confirmButton.IsActive <- false
            | Some fileData ->
                let index = this.Files.IndexOf fileData

                match index with
                | -1 -> _currentSelection <- None
                | _ ->
                    _currentSelection <- Some index
                    _confirmButton.IsActive <- true

    member this.SelectAt index =
        if index < 0 || index >= this.Files.Count then
            _currentSelection <- None
            _confirmButton.IsActive <- false
        else
            _currentSelection <- Some index
            _confirmButton.IsActive <- true

    member this.ClearSelection() =
        this.CurrentSelection <- None
        _confirmButton.IsActive <- false

    member this.ProcessMouseInput() =
        let mouseScroll = Mouse.getScrollAmount ()
        this.Camera.MoveCameraDown(-mouseScroll.Y * this.FileEntryHeight)

        if this.Camera.Position.Y < 0.0f then
            this.Camera.MoveCameraUp(-mouseScroll.Y * this.FileEntryHeight)

        let struct (startY, endY) = this.CalculateVisibleIndexRange()

        let maxCameraPosition =
            float32 this.Files.Count * this.FileEntryHeight - this.Height

        if this.Camera.Position.Y > maxCameraPosition then
            this.Camera.MoveCameraDown(mouseScroll.Y * this.FileEntryHeight)

        if
            Mouse.buttonClicked MouseButton.Left
            && not (
                Button.isClicked this.ConfirmButton
                || Button.isClicked this.CancelButton
                || Button.isPressed this.ConfirmButton
                || Button.isPressed this.CancelButton
            )
        then
            let mousePos = Mouse.getPosition () + this.Camera.Position - Vector2(this.X, this.Y)
            this.ClearSelection()

            for index = startY to endY do
                let minX = this.X
                let maxX = this.X + this.FileEntryWidth
                let minY = this.Y + this.FileEntryHeight * float32 index
                let maxY = minY + this.FileEntryHeight

                if
                    mousePos.X >= float32 minX
                    && mousePos.X <= float32 maxX
                    && mousePos.Y >= float32 minY
                    && mousePos.Y <= float32 maxY
                then
                    this.SelectAt index
        else if Mouse.buttonClicked MouseButton.Right then
            this.ClearSelection()

    member this.ProcessKeyboardInput() =
        match _currentSelection with
        | None -> ()
        | Some selectedIndex ->
            let struct (startY, endY) = this.CalculateVisibleIndexRange()

            if Keyboard.keyHasBeenPressedOnce KeyboardKey.Down then
                let newIndex = min (selectedIndex + 1) (this.Files.Count - 1)
                _currentSelection <- Some newIndex

                if newIndex > endY then
                    this.Camera.MoveCameraDown this.FileEntryHeight

            else if Keyboard.keyHasBeenPressedOnce KeyboardKey.Up then
                let newIndex = max (selectedIndex - 1) 0
                _currentSelection <- Some newIndex

                if newIndex < startY then
                    this.Camera.MoveCameraUp this.FileEntryHeight

    member private this.ProcessButton(button: Button) =
        match button.IsActive with
        | false -> ()
        | true ->
            if Button.isClicked button || Button.isShortcutPressed button then
                this.ActivatedButton <- Some button

            match this.ActivatedButton with
            | None ->
                if Button.isPressed button then
                    this.ActivatedButton <- Some button

                    match button.OnPressAndHold with
                    | Some callback -> callback ()
                    | None -> ()
            | Some pressedButton when pressedButton = button ->
                if Button.isPressed button then
                    match button.OnPressAndHold with
                    | Some callback -> callback ()
                    | None -> ()
            | Some _ -> ()

            match this.ActivatedButton with
            | None -> ()
            | Some activatedButton ->
                if Button.isReleased activatedButton then
                    this.ActivatedButton <- None

    member this.CalculateVisibleIndexRange() =
        let trueY = this.Camera.Position.Y - this.Y

        let startIndex = max (int trueY / int this.FileEntryHeight) 0

        let endIndex =
            min ((int trueY + int this.Height) / int this.FileEntryHeight) (this.Files.Count - 1)

        struct (startIndex, endIndex)

    member this.ProcessInput() =
        this.ProcessButton _confirmButton
        this.ProcessButton _cancelButton
        this.ProcessMouseInput()
        this.ProcessKeyboardInput()
