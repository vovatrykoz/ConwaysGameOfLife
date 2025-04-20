namespace Conway.App.Raylib

type ControlManager() =

    member val private ActivatedButton: option<Button> = None with get, set

    member val Buttons = seq<Button> Seq.empty with get, set

    member val GridControls = seq<GridControl> Seq.empty with get, set

    member this.AddButton(button: Button) =
        this.Buttons <- seq { button } |> Seq.append this.Buttons

    member this.AddButtons buttons =
        this.Buttons <- buttons |> Seq.append this.Buttons

    member this.AddGridControl(gridControl: GridControl) =
        this.GridControls <- seq { gridControl } |> Seq.append this.GridControls

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

    member private this.ProcessGridControls() =
        this.GridControls
        |> Seq.iter (fun gridControl ->
            GridControl.IsLeftPressed gridControl |> ignore
            GridControl.IsRightPressed gridControl |> ignore)

    member this.ReadInput() =
        this.ProcessButtons()
        this.ProcessGridControls()

    member this.UpdateControls() =
        this.Buttons
        |> Seq.iter (fun button ->
            match button.OnUpdate with
            | Some updateCallback -> updateCallback button
            | None -> ())
