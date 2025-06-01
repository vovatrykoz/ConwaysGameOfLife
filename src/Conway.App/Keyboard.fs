namespace Conway.App

open Conway.App.Aliases
open Raylib_cs

module Keyboard =
    let inline keyIsDown key = raylibTrue (Raylib.IsKeyDown key)

    let inline keyHasBeenPressedOnce key = raylibTrue (Raylib.IsKeyPressed key)
