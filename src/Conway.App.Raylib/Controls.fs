namespace Conway.App.Raylib

open Raylib_cs

type ControlManager() =

    member val private ActivatedButtons: list<Button> = List.empty with get, set

    member val Buttons = seq<Button> Seq.empty with get, set

    member this.AddButton(button: Button) =
        this.Buttons <- seq { button } |> Seq.append this.Buttons

    member this.ReadInput() =
        this.Buttons
        |> Seq.iter (fun button ->
            match button.IsActive with
            | false -> ()
            | true ->
                if Button.isClicked button then
                    match button.OnClick with
                    | Some callback -> callback ()
                    | None -> ()

                if not (this.ActivatedButtons |> List.contains button) then
                    if Button.isPressed button then
                        this.ActivatedButtons <- button :: this.ActivatedButtons

                        match button.OnPressAndHold with
                        | Some callback -> callback ()
                        | None -> ()
                else
                    ()

                if Mouse.buttonIsUp MouseButton.Left then
                    this.ActivatedButtons <- List.empty)

    member this.UpdateControls() =
        this.Buttons
        |> Seq.iter (fun button ->
            match button.Update with
            | Some updateCallback -> updateCallback button
            | None -> ())

    member this.ReadUserInput keysToProcess =
        Keyboard.readKeyPresses keysToProcess
        this.ReadInput()
