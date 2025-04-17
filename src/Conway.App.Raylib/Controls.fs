namespace Conway.App.Raylib

type Button = {
    X: int
    Y: int
    Size: int
    Text: string
    OnClick: unit -> unit
    Update: Button -> Button
} with

    static member create x y size text onClick update = {
        X = x
        Y = y
        Size = size
        Text = text
        OnClick = onClick
        Update = update
    }

module private Utils =
    open System.Numerics

    let isPressed button leftMouseButtonIsPressed (getMousePosition: unit -> Vector2) =
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

type ControlManager = {
    Buttons: seq<Button>
} with

    static member createEmpty = { Buttons = Seq.empty }

    static member create buttons = { Buttons = buttons }

    static member addButton (button: Button) controlManager = {
        controlManager with
            Buttons = seq { button } |> Seq.append controlManager.Buttons
    }

    static member checkMouseInput leftMouseButtonIsPressed getMousePosition controlManager =
        controlManager.Buttons
        |> Seq.iter (fun button ->
            if Utils.isPressed button leftMouseButtonIsPressed getMousePosition then
                button.OnClick())

    static member update controlManager =
        ControlManager.create (controlManager.Buttons |> Seq.map (fun button -> button.Update button))
