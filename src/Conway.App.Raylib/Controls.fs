namespace Conway.App.Raylib

open Raylib_cs
open System.Numerics

type Button
    (
        x: int,
        y: int,
        size: int,
        text: string,
        isActive: bool,
        isVisible: bool,
        onClick: option<unit -> unit>,
        onPressAndHold: option<unit -> unit>,
        update: option<Button -> unit>
    ) =

    let mutable isCurrentlyPressed = false

    let mutable isCurrentlyClicked = false

    let mutable isActive = isActive

    member _.IsPressed
        with get () = isCurrentlyPressed
        and private set value = isCurrentlyPressed <- value

    member _.IsClicked
        with get () = isCurrentlyClicked
        and private set value = isCurrentlyClicked <- value

    member val X = x with get, set

    member val Y = y with get, set

    member val Size = size with get, set

    member val Text = text with get, set

    member _.IsActive
        with get () = isActive
        and set value =
            if not value then
                isCurrentlyPressed <- false

            isActive <- value

    member val IsVisible = isVisible with get, set

    member val OnClick = onClick with get, set

    member val OnPressAndHold = onPressAndHold with get, set

    member val Update = update with get, set

    static member internal isPressed(button: Button) =
        if Mouse.readButtonPress MouseButton.Left then
            let mousePos = Mouse.getPosition ()
            let minX = button.X
            let maxX = button.X + button.Size
            let minY = button.Y
            let maxY = button.Y + button.Size

            if
                mousePos.X >= float32 minX
                && mousePos.X <= float32 maxX
                && mousePos.Y >= float32 minY
                && mousePos.Y <= float32 maxY
            then
                button.IsPressed <- true
                button.IsPressed
            else
                button.IsPressed <- false
                button.IsPressed
        else
            button.IsPressed <- false
            button.IsPressed

    static member internal isClicked(button: Button) =
        if Mouse.readButtonClick MouseButton.Left then
            let mousePos = Mouse.getPosition ()
            let minX = button.X
            let maxX = button.X + button.Size
            let minY = button.Y
            let maxY = button.Y + button.Size

            if
                mousePos.X >= float32 minX
                && mousePos.X <= float32 maxX
                && mousePos.Y >= float32 minY
                && mousePos.Y <= float32 maxY
            then
                button.IsClicked <- true
                button.IsClicked
            else
                button.IsClicked <- false
                button.IsClicked
        else
            button.IsClicked <- false
            button.IsClicked

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

                if Button.isPressed button then
                    if not (this.ActivatedButtons |> List.contains button) then
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
