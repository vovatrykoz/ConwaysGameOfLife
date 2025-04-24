namespace Conway.App.Raylib

open Conway.App.Raylib.Aliases
open Raylib_cs

module Keyboard =
    let readKeyDown key = raylibTrue (Raylib.IsKeyDown key)

    let readKeyPress key = raylibTrue (Raylib.IsKeyPressed key)
