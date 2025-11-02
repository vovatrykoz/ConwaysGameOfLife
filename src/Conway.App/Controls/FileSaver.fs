namespace Conway.App.Controls

open Conway.App.Math
open Raylib_cs
open System.Text

type FileSaver
    (
        x: float32<px>,
        y: float32<px>,
        width: float32<px>,
        height: float32<px>,
        fileEntryWidth: float32<px>,
        fileEntryHeight: float32<px>
    ) =

    let mutable _cancelled = false

    let mutable _confirmed = false

    let _confirmButton =
        new Button(
            int x,
            int (y + fileEntryHeight * 15.0f),
            200,
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
            int x + 300,
            int (y + fileEntryHeight * 15.0f),
            200,
            40,
            "Cancel",
            true,
            true,
            Some(fun _ -> _cancelled <- true),
            None,
            None,
            Some KeyboardKey.Escape
        )

    member val X = x with get, set

    member val Y = y with get, set

    member val Width = width with get, set

    member val Height = height with get, set

    member val FileEntryWidth = fileEntryWidth with get, set

    member val FileEntryHeight = fileEntryHeight with get, set

    member val private ActivatedButton: option<Button> = None with get, set

    member _.Cancelled
        with get () = _cancelled
        and private set value = _cancelled <- value

    member _.Confirmed
        with get () = _confirmed
        and private set value = _confirmed <- value

    member val Buffer = new StringBuilder() with get

    member _.ConfirmButton = _confirmButton

    member _.CancelButton = _cancelButton

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

    member this.ProcessInput() =
        this.ProcessButton _confirmButton
        this.ProcessButton _cancelButton

        if this.Buffer.Length > 0 then
            this.ConfirmButton.IsActive <- true
        else
            this.ConfirmButton.IsActive <- false
