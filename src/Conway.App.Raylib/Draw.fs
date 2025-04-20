namespace Conway.App.Raylib

open Raylib_cs

module Draw =
    let private width = 25

    let private height = 25

    let border x y width height color =
        Raylib.DrawRectangleLines(x * width, y * height, width, height, color)

    let cell x y width height color =
        border x y width height Color.Black
        Raylib.DrawRectangle(x * width + 1, y * height + 1, width - 2, height - 2, color)

    let livingCell x y width height = cell x y width height Color.Red

    let deadCell x y width height = cell x y width height Color.Black

    let button (button: Button) =
        match button.IsActive with
        | true -> Raylib.DrawRectangle(button.X, button.Y, button.Size, button.Size, Color.Black)
        | false -> Raylib.DrawRectangle(button.X, button.Y, button.Size, button.Size, Color.Gray)

        Raylib.DrawText(button.Text, button.X + 5, button.Y + 5, 15, Color.White)

        match button.IsPressed with
        | false -> ()
        | true ->
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

    let textBox (text: string) =
        Raylib.DrawText(text, 600, 50, 24, Color.Black)
