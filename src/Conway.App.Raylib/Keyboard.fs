namespace Conway.App.Raylib

open Conway.App.Raylib.Aliases
open Raylib_cs

module Keyboard =
    let readSpacePress callback =
        if raylibTrue (Raylib.IsKeyPressed KeyboardKey.Space) then
            callback ()

    let readRightArrowKey callback =
        if raylibTrue (Raylib.IsKeyPressed KeyboardKey.Right) then
            callback ()

    let readKeyPresses keyCallbackPairs =
        keyCallbackPairs
        |> Seq.iter (fun (keyPressFunc, callback) -> keyPressFunc callback)
