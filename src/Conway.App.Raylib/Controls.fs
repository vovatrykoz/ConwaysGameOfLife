namespace Conway.App.Raylib

open Conway.Core
open Raylib_cs

type ControlManager(game: Game) =

    member val private ActivatedButton: option<Button> = None with get, set

    member val Buttons = seq<Button> Seq.empty with get, set

    member val Canvas = new Canvas(0, 0, game, 25, 1) with get, set

    member val KeyActions = seq<KeyboardKey * (unit -> unit)> Seq.empty with get, set

    member this.AddButton(button: Button) =
        this.Buttons <- seq { button } |> Seq.append this.Buttons

    member this.AddButtons buttons =
        this.Buttons <- buttons |> Seq.append this.Buttons

    member this.AddKeyAction (key: KeyboardKey) (action: unit -> unit) =
        this.KeyActions <- seq { key, action } |> Seq.append this.KeyActions

    member private this.ProcessButtons() =
        this.Buttons
        |> Seq.iter (fun button ->
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
        |> Seq.iter (fun (key, action) ->
            if Keyboard.readKeyPress key then
                action ())

    member private this.ProcessGridControls() =
        this.Canvas.Controls
        |> Seq.iter (fun gridControl ->
            CanvasControl.IsLeftPressed gridControl |> ignore
            CanvasControl.IsRightPressed gridControl |> ignore)

    member this.ReadInput() =
        this.ProcessButtons()
        this.ProcessKeyActions()
        this.ProcessGridControls()

    member this.UpdateControls() =
        this.Buttons
        |> Seq.iter (fun button ->
            match button.OnUpdate with
            | Some updateCallback -> updateCallback button
            | None -> ())
