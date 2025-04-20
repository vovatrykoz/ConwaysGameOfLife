namespace Conway.App.Raylib

open Raylib_cs

type GridControl(x: int, y: int, size: int, onLeftClick: option<unit -> unit>, onRightClick: option<unit -> unit>) =
    member val X = x with get, set

    member val Y = y with get, set

    member val Size = size with get, set

    member val OnLeftClick = onLeftClick with get, set

    member val OnRightClick = onRightClick with get, set

type ControlManager() =

    member val private ActivatedButton: option<Button> = None with get, set

    member val Buttons = seq<Button> Seq.empty with get, set

    member val GridControls = seq<GridControl> Seq.empty with get, set

    member this.AddButton(button: Button) =
        this.Buttons <- seq { button } |> Seq.append this.Buttons

    member this.AddGridControl(gridControl: GridControl) =
        this.GridControls <- seq { gridControl } |> Seq.append this.GridControls

    member private this.ProcessButtons() =
        this.Buttons
        |> Seq.iter (fun button ->
            match button.IsActive with
            | false -> ()
            | true ->
                if Button.isClicked button then
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

    member private this.ProcessGridControls() =
        this.GridControls
        |> Seq.iter (fun gridControl ->
            if Mouse.readButtonPress MouseButton.Left then
                match gridControl.OnLeftClick with
                | None -> ()
                | Some callback -> callback ()

            if Mouse.readButtonPress MouseButton.Right then
                match gridControl.OnRightClick with
                | None -> ()
                | Some callback -> callback ())

    member this.ReadInput() =
        this.ProcessButtons()
        this.ProcessGridControls()

    member this.UpdateControls() =
        this.Buttons
        |> Seq.iter (fun button ->
            match button.Update with
            | Some updateCallback -> updateCallback button
            | None -> ())

    member this.ReadUserInput keysToProcess =
        Keyboard.readKeyPresses keysToProcess
        this.ReadInput()
