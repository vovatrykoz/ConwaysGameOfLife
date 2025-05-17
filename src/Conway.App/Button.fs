namespace Conway.App

open Raylib_cs

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
        onUpdate: option<Button -> unit>,
        shortcut: option<KeyboardKey>
    ) =

    let mutable _isCurrentlyPressed = false

    let mutable _isCurrentlyClicked = false

    let mutable _isActive = isActive

    member _.IsPressed
        with get () = _isCurrentlyPressed
        and private set value = _isCurrentlyPressed <- value

    member _.IsClicked
        with get () = _isCurrentlyClicked
        and private set value = _isCurrentlyClicked <- value

    member val X = x with get, set

    member val Y = y with get, set

    member val Size = size with get, set

    member val Text = text with get, set

    member _.IsActive
        with get () = _isActive
        and set value =
            if not value then
                _isCurrentlyPressed <- false

            _isActive <- value

    member val IsVisible = isVisible with get, set

    member val OnClick = onClick with get, set

    member val OnPressAndHold = onPressAndHold with get, set

    member val OnUpdate = onUpdate with get, set

    member val Shortcut = shortcut with get, set

    static member isShortcutPressed(button: Button) =
        match button.Shortcut with
        | None ->
            button.IsPressed <- false
            button.IsPressed
        | Some shortcut ->
            if Keyboard.keyHasBeenPressedOnce shortcut then
                button.IsPressed <- true

                match button.OnClick with
                | None -> ()
                | Some callback -> callback ()

                button.IsPressed
            else
                button.IsPressed <- false
                button.IsPressed

    static member isPressed(button: Button) =
        if Mouse.buttonIsPressed MouseButton.Left then
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
                || button.IsPressed
            then
                button.IsPressed <- true
                button.IsPressed
            else
                button.IsPressed <- false
                button.IsPressed
        else
            button.IsPressed <- false
            button.IsPressed

    static member isClicked(button: Button) =
        if Mouse.buttonClicked MouseButton.Left then
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

    static member isReleased(button: Button) =
        if not (Mouse.buttonHasBeenReleased MouseButton.Left) then
            false
        else
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
                match button.OnClick with
                | None -> ()
                | Some callback -> callback ()

                true
            else
                false

    static member create = new Button(0, 0, 0, "", true, true, None, None, None, None)

    static member position x y (button: Button) =
        button.X <- x
        button.Y <- y
        button

    static member size size (button: Button) =
        button.Size <- size
        button

    static member text text (button: Button) =
        button.Text <- text
        button

    static member activate(button: Button) =
        button.IsActive <- true
        button

    static member deactivate(button: Button) =
        button.IsActive <- false
        button

    static member show(button: Button) =
        button.IsVisible <- true
        button

    static member hide(button: Button) =
        button.IsVisible <- false
        button

    static member onClickCallback callback (button: Button) =
        button.OnClick <- Some callback
        button

    static member onPressAndHoldCallback callback (button: Button) =
        button.OnPressAndHold <- Some callback
        button

    static member onUpdateCallback callback (button: Button) =
        button.OnUpdate <- Some callback
        button

    static member shortcut shortcut (button: Button) =
        button.Shortcut <- Some shortcut
        button
