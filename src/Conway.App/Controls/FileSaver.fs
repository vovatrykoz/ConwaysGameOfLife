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
        let x_px: int<px> = LanguagePrimitives.Int32WithMeasure(int x)

        let y_px: int<px> =
            LanguagePrimitives.Int32WithMeasure(int (y + fileEntryHeight * 15.0f))

        new Button(
            x = x_px,
            y = y_px,
            width = 200<px>,
            height = 40<px>,
            text = "Confirm",
            isActive = false,
            isVisible = true,
            onClick = Some(fun _ -> _confirmed <- true),
            onPressAndHold = None,
            onUpdate = None,
            shortcut = Some KeyboardKey.Enter
        )

    let _cancelButton =
        let x_px: int<px> = LanguagePrimitives.Int32WithMeasure(int x)

        let y_px: int<px> =
            LanguagePrimitives.Int32WithMeasure(int (y + fileEntryHeight * 15.0f))

        new Button(
            x_px + 300<px>,
            y_px,
            200<px>,
            40<px>,
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
