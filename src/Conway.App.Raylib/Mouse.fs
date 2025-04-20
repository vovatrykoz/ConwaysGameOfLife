namespace Conway.App.Raylib

open Conway.App.Raylib.Aliases
open Raylib_cs

module Mouse =
    let readButtonClick key =
        raylibTrue (Raylib.IsMouseButtonPressed key)

    let readButtonPress key =
        raylibTrue (Raylib.IsMouseButtonDown key)

    let buttonIsUp key = raylibTrue (Raylib.IsMouseButtonUp key)

    let buttonHasBeenReleased key =
        raylibTrue (Raylib.IsMouseButtonReleased key)

    let getPosition () = Raylib.GetMousePosition()
