namespace Conway.App.Controls

open Conway.App.Input
open Conway.App.Math
open Raylib_cs

/// <summary>
/// Represents a UI button that can respond to mouse clicks, key shortcuts, and custom update logic.
/// </summary>
/// <param name="x">The x-coordinate of the button in pixels.</param>
/// <param name="y">The y-coordinate of the button in pixels.</param>
/// <param name="width">The width of the button in pixels.</param>
/// <param name="height">The height of the button in pixels.</param>
/// <param name="text">The text displayed on the button.</param>
/// <param name="isActive">Indicates whether the button is currently active (interactive).</param>
/// <param name="isVisible">Indicates whether the button is visible.</param>
/// <param name="onClick">Optional callback invoked when the button is clicked.</param>
/// <param name="onPressAndHold">Optional callback invoked while the button is pressed and held.</param>
/// <param name="onUpdate">Optional callback invoked on each update, receiving the button instance.</param>
/// <param name="shortcut">Optional keyboard shortcut that can trigger the button.</param>
type Button
    (
        x: int<px>,
        y: int<px>,
        width: int<px>,
        height: int<px>,
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

    /// <summary>Indicates whether the button is currently pressed.</summary>
    member _.IsPressed
        with get () = _isCurrentlyPressed
        and private set value = _isCurrentlyPressed <- value

    /// <summary>Indicates whether the button has been clicked.</summary>
    member _.IsClicked
        with get () = _isCurrentlyClicked
        and private set value = _isCurrentlyClicked <- value

    /// <summary>X-coordinate of the button in pixels.</summary>
    member val X = x with get, set

    /// <summary>Y-coordinate of the button in pixels.</summary>
    member val Y = y with get, set

    /// <summary>Width of the button in pixels.</summary>
    member val Width = width with get, set

    /// <summary>Height of the button in pixels.</summary>
    member val Height = height with get, set

    /// <summary>Text displayed on the button.</summary>
    member val Text = text with get, set

    /// <summary>Indicates whether the button is active. Deactivating it also resets its pressed state.</summary>
    member _.IsActive
        with get () = _isActive
        and set value =
            if not value then
                _isCurrentlyPressed <- false

            _isActive <- value

    /// <summary>Indicates whether the button is visible.</summary>
    member val IsVisible = isVisible with get, set

    /// <summary>Callback invoked when the button is clicked.</summary>
    member val OnClick = onClick with get, set

    /// <summary>Callback invoked while the button is pressed and held.</summary>
    member val OnPressAndHold = onPressAndHold with get, set

    /// <summary>Callback invoked on each update with the button instance.</summary>
    member val OnUpdate = onUpdate with get, set

    /// <summary>Optional keyboard shortcut that can trigger the button.</summary>
    member val Shortcut = shortcut with get, set

    /// <summary>
    /// Checks if the keyboard shortcut assigned to the button has been pressed.
    /// Executes the OnClick callback if triggered.
    /// </summary>
    /// <param name="button">The button instance to check.</param>
    /// <returns>True if the shortcut was pressed; otherwise, false.</returns>
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

    /// <summary>
    /// Checks if the button is currently being pressed with the mouse.
    /// </summary>
    /// <param name="button">The button instance to check.</param>
    /// <returns>True if the button is pressed; otherwise, false.</returns>
    static member isPressed(button: Button) =
        if Mouse.buttonIsPressed MouseButton.Left then
            let mousePos = Mouse.getPosition ()
            let minX = button.X
            let maxX = button.X + button.Width
            let minY = button.Y
            let maxY = button.Y + button.Height

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

    /// <summary>
    /// Checks if the button has been clicked with the mouse.
    /// </summary>
    /// <param name="button">The button instance to check.</param>
    /// <returns>True if the button was clicked; otherwise, false.</returns>
    static member isClicked(button: Button) =
        if Mouse.buttonClicked MouseButton.Left then
            let mousePos = Mouse.getPosition ()
            let minX = button.X
            let maxX = button.X + button.Width
            let minY = button.Y
            let maxY = button.Y + button.Height

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

    /// <summary>
    /// Checks if the mouse button has been released over the button.
    /// Executes the OnClick callback if released inside the button area.
    /// </summary>
    /// <param name="button">The button instance to check.</param>
    /// <returns>True if the button was released over the button; otherwise, false.</returns>
    static member isReleased(button: Button) =
        if not (Mouse.buttonHasBeenReleased MouseButton.Left) then
            false
        else
            let mousePos = Mouse.getPosition ()
            let minX = button.X
            let maxX = button.X + button.Width
            let minY = button.Y
            let maxY = button.Y + button.Height

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

    /// <summary>Creates a new default button instance with no size, text, or callbacks.</summary>
    static member create =
        new Button(0<px>, 0<px>, 0<px>, 0<px>, "", true, true, None, None, None, None)

    /// <summary>Sets the position of the button.</summary>
    static member position x y (button: Button) =
        button.X <- x
        button.Y <- y
        button

    /// <summary>Sets the width of the button.</summary>
    static member width width (button: Button) =
        button.Width <- width
        button

    /// <summary>Sets the height of the button.</summary>
    static member height height (button: Button) =
        button.Height <- height
        button

    /// <summary>Sets the text of the button.</summary>
    static member text text (button: Button) =
        button.Text <- text
        button

    /// <summary>Activates the button.</summary>
    static member activate(button: Button) =
        button.IsActive <- true
        button

    /// <summary>Deactivates the button.</summary>
    static member deactivate(button: Button) =
        button.IsActive <- false
        button

    /// <summary>Makes the button visible.</summary>
    static member show(button: Button) =
        button.IsVisible <- true
        button

    /// <summary>Makes the button invisible.</summary>
    static member hide(button: Button) =
        button.IsVisible <- false
        button

    /// <summary>Sets the OnClick callback for the button.</summary>
    static member onClickCallback callback (button: Button) =
        button.OnClick <- Some callback
        button

    /// <summary>Sets the OnPressAndHold callback for the button.</summary>
    static member onPressAndHoldCallback callback (button: Button) =
        button.OnPressAndHold <- Some callback
        button

    /// <summary>Sets the OnUpdate callback for the button.</summary>
    static member onUpdateCallback callback (button: Button) =
        button.OnUpdate <- Some callback
        button

    /// <summary>Assigns a keyboard shortcut to the button.</summary>
    static member shortcut shortcut (button: Button) =
        button.Shortcut <- Some shortcut
        button
