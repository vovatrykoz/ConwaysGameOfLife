namespace Conway.App.Raylib

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

    let button (button: Button) =
        match button.IsActive with
        | true -> Raylib.DrawRectangle(button.X, button.Y, button.Size, button.Size, Color.Black)
        | false -> Raylib.DrawRectangle(button.X, button.Y, button.Size, button.Size, Color.Gray)

        Raylib.DrawText(button.Text, button.X + 5, button.Y + 5, 15, Color.White)

        match button.IsPressed, button.IsActive with
        | false, _
        | _, false -> ()
        | true, true ->
            Raylib.DrawRectangleLinesEx(
                new Rectangle(
                    float32 (button.X - 2),
                    float32 (button.Y - 2),
                    float32 (button.Size + 4),
                    float32 (button.Size + 4)
                ),
                2.0f,
                Color.Black
            )
