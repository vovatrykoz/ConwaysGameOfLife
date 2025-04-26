namespace Conway.App.Raylib

open Conway.App.Raylib.Aliases
open Raylib_cs

module Keyboard =
    let keyIsDown key = raylibTrue (Raylib.IsKeyDown key)

    let keyHasBeenPressedOnce key = raylibTrue (Raylib.IsKeyPressed key)
