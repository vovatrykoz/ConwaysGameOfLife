namespace Conway.App.Raylib

open System.Collections.Generic
open Raylib_cs

type ControlManager(canvas: Canvas) =

    member val private ActivatedButton: option<Button> = None with get, set

    member val Buttons: List<Button> = new List<Button>() with get, set

    member val Canvas = canvas with get, set

    member val KeyActions: List<KeyboardKey * (unit -> unit)> = new List<KeyboardKey * (unit -> unit)>() with get, set

    member private this.ProcessButtons() =
        for button in this.Buttons do
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

    member private this.ProcessKeyActions() =
        for key, action in this.KeyActions do
            if Keyboard.readKeyPress key then
                action ()

    member this.ReadInput() =
        this.ProcessButtons()
        this.ProcessKeyActions()
        this.Canvas.ProcessDrawableArea()

    member this.UpdateControls() =
        for button in this.Buttons do
            match button.OnUpdate with
            | Some updateCallback -> updateCallback button
            | None -> ()
