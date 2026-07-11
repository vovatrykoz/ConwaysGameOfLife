namespace Conway.App.Input

module Keyboard =
    open Conway.App.Utils.Alias
    open Raylib_cs

    let inline keyIsDown key = raylibTrue (Raylib.IsKeyDown key)

    let inline keyHasBeenPressedOnce key = raylibTrue (Raylib.IsKeyPressed key)

    let inline getKeyPressed () = Raylib.GetKeyPressed()

    let inline getCharPressed () = Raylib.GetCharPressed()
