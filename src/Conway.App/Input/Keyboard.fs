namespace Conway.App.Input

open Conway.App.Utils.Alias
open Raylib_cs

module Keyboard =
    let inline keyIsDown key = raylibTrue (Raylib.IsKeyDown key)

    let inline keyHasBeenPressedOnce key = raylibTrue (Raylib.IsKeyPressed key)

    let inline getKeyPressed () = Raylib.GetKeyPressed()

    let inline getCharPressed () = Raylib.GetCharPressed()
