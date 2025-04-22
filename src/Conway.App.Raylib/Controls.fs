namespace Conway.App.Raylib

open Conway.Core
open Raylib_cs

type ControlManager(canvas: Canvas) =

    member val private ActivatedButton: option<Button> = None with get, set

    member val Buttons: list<Button> = List.empty with get, set

    member val Canvas = canvas with get, set

    member val KeyActions: list<KeyboardKey * (unit -> unit)> = List.empty with get, set

    member this.AddButton(button: Button) = this.Buttons <- button :: this.Buttons

    member this.AddButtons buttons =
        this.Buttons <- this.Buttons |> List.append buttons

    member this.AddKeyAction (key: KeyboardKey) (action: unit -> unit) =
        this.KeyActions <- (key, action) :: this.KeyActions

    member private this.ProcessButtons() =
        this.Buttons
        |> List.iter (fun button ->
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
                        this.ActivatedButton <- None)

    member private this.ProcessKeyActions() =
        this.KeyActions
        |> List.iter (fun (key, action) ->
            if Keyboard.readKeyPress key then
                action ())

    member this.ReadInput() =
        this.ProcessButtons()
        this.ProcessKeyActions()
        this.Canvas.ProcessDrawableArea()

    member this.UpdateControls() =
        this.Buttons
        |> List.iter (fun button ->
            match button.OnUpdate with
            | Some updateCallback -> updateCallback button
            | None -> ())
