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

    let button button =
        Raylib.DrawRectangle(button.X, button.Y, button.Size, button.Size, Color.Black)
        Raylib.DrawText(button.Text, button.X + 5, button.Y + 5, 15, Color.White)
