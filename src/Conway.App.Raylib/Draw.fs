namespace Conway.App.Raylib

open Conway.Core
open Raylib_cs

module Draw =
    let private size = 25

    let border x y color =
        Raylib.DrawRectangleLines(x * size, y * size, size, size, color)

    let cell x y color =
        border x y Color.Black
        Raylib.DrawRectangle(x * size + 1, y * size + 1, size - 2, size - 2, color)

    let livingCell x y = cell x y Color.Red

    let deadCell x y = cell x y Color.Black

    let runButton x y state =
        let buttonText =
            match state with
            | Infinite
            | Limited _ -> "Pause"
            | Paused -> "Run"

        Raylib.DrawRectangle(x, y, size * 2, size * 2, Color.Black)
        Raylib.DrawText(buttonText, x + 5, y + 5, 15, Color.White)
