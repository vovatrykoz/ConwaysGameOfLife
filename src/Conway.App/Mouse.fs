namespace Conway.App

open Conway.App.Aliases
open Raylib_cs

module Mouse =
    let inline position () = Raylib.GetMousePosition()

    let inline getDelta () = Raylib.GetMouseDelta()

    let inline getScrollAmount () = Raylib.GetMouseWheelMoveV()

    let inline buttonClicked key =
        raylibTrue (Raylib.IsMouseButtonPressed key)

    let inline buttonIsPressed key =
        raylibTrue (Raylib.IsMouseButtonDown key)

    let inline buttonIsUp key = raylibTrue (Raylib.IsMouseButtonUp key)

    let inline buttonHasBeenReleased key =
        raylibTrue (Raylib.IsMouseButtonReleased key)

    let inline getPosition () = Raylib.GetMousePosition()
