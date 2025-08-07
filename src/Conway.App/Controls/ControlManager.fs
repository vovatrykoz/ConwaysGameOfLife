namespace Conway.App.Controls

open Conway.App.Input
open System.Collections.Generic
open Raylib_cs

type ControlManager() =

    member val private ActivatedButton: option<Button> = None with get, set

    member val Buttons: List<Button> = new List<Button>() with get, set

    member val KeyActions: List<KeyboardKey * (unit -> unit)> = new List<KeyboardKey * (unit -> unit)>() with get, set

    member val ShiftKeyActions: List<KeyboardKey * (unit -> unit)> =
        new List<KeyboardKey * (unit -> unit)>() with get, set

    member val CtrlKeyActions: List<KeyboardKey * (unit -> unit)> =
        new List<KeyboardKey * (unit -> unit)>() with get, set

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
        if
            Keyboard.keyIsDown KeyboardKey.LeftShift
            || Keyboard.keyIsDown KeyboardKey.RightShift
        then
            for key, action in this.ShiftKeyActions do
                if Keyboard.keyHasBeenPressedOnce key then
                    action ()
        else if
            Keyboard.keyIsDown KeyboardKey.LeftControl
            || Keyboard.keyIsDown KeyboardKey.RightControl
        then
            for key, action in this.CtrlKeyActions do
                if Keyboard.keyHasBeenPressedOnce key then
                    action ()
        else
            for key, action in this.KeyActions do
                if Keyboard.keyHasBeenPressedOnce key then
                    action ()

    member this.ReadInput() =
        this.ProcessButtons()
        this.ProcessKeyActions()

    member this.UpdateControls() =
        for button in this.Buttons do
            match button.OnUpdate with
            | Some updateCallback -> updateCallback button
            | None -> ()
