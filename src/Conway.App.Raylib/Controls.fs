namespace Conway.App.Raylib

open System.Numerics

type Button
    (
        x: int,
        y: int,
        size: int,
        text: string,
        isActive: bool,
        isVisible: bool,
        onPressAndHold: option<unit -> unit>,
        update: option<Button -> unit>
    ) =

    let mutable isCurrentlyPressed = false

    let mutable isActive = isActive

    member _.IsPressed
        with get () = isCurrentlyPressed
        and private set value = isCurrentlyPressed <- value

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

    member val OnPressAndHold = onPressAndHold with get, set

    member val Update = update with get, set

    static member internal isPressed (button: Button) leftMouseButtonIsPressed (getMousePosition: unit -> Vector2) =
        if leftMouseButtonIsPressed () then
            let mousePos = getMousePosition ()
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
                true
            else
                button.IsPressed <- false
                false
        else
            button.IsPressed <- false
            false

type ControlManager() =

    member val private ActivatedButtons: list<Button> = List.empty with get, set

    member val Buttons = seq<Button> Seq.empty with get, set

    member this.AddButton(button: Button) =
        this.Buttons <- seq { button } |> Seq.append this.Buttons

    member this.ReadInput leftMouseButtonIsPressed leftMouseButtonIsUp getMousePosition =
        this.Buttons
        |> Seq.iter (fun button ->
            match button.IsActive with
            | false -> ()
            | true ->
                if Button.isPressed button leftMouseButtonIsPressed getMousePosition then
                    if not (this.ActivatedButtons |> List.contains button) then
                        this.ActivatedButtons <- button :: this.ActivatedButtons

                        printfn $"Here, len {List.length this.ActivatedButtons} button {button.X} {button.Y}"

                        match button.OnPressAndHold with
                        | Some callback -> callback ()
                        | None -> ()
                    else
                        ()

                if leftMouseButtonIsUp () then
                    this.ActivatedButtons <- List.empty)

    member this.UpdateControls() =
        this.Buttons
        |> Seq.iter (fun button ->
            match button.Update with
            | Some updateCallback -> updateCallback button
            | None -> ())

    member this.ReadUserInput keysToProcess lmbFunc mousePosFunc =
        Keyboard.readKeyPresses keysToProcess
        this.ReadInput lmbFunc mousePosFunc
