namespace Conway.App.Raylib

type Button(x: int, y: int, size: int, text: string, onClick: option<unit -> unit>, update: option<Button -> unit>) =

    member val X = x with get, set

    member val Y = y with get, set

    member val Size = size with get, set

    member val Text = text with get, set

    member val OnClick = onClick with get, set

    member val Update = update with get, set

module private Utils =
    open System.Numerics

    let isPressed (button: Button) leftMouseButtonIsPressed (getMousePosition: unit -> Vector2) =
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
                true
            else
                false
        else
            false

type ControlManager() =

    member val Buttons = seq<Button> Seq.empty with get, set

    member this.AddButton(button: Button) =
        this.Buttons <- seq { button } |> Seq.append this.Buttons

    member this.ReadInput leftMouseButtonIsPressed getMousePosition =
        this.Buttons
        |> Seq.iter (fun button ->
            if Utils.isPressed button leftMouseButtonIsPressed getMousePosition then
                match button.OnClick with
                | Some callback -> callback ()
                | None -> ())

    member this.Update() =
        this.Buttons
        |> Seq.iter (fun button ->
            match button.Update with
            | Some updateCallback -> updateCallback button
            | None -> ())
