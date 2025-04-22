namespace Conway.App.Raylib

open Raylib_cs

module Draw =
    let private buttonBorderRectangle x y size =
        Rectangle(float32 (x - 2), float32 (y - 2), float32 (size + 4), float32 (size + 4))

    let inline private calculateBorderSize buttonSize = float32 buttonSize / 12.5f

    let border x y width height thickness color =
        let rectangle =
            Rectangle(float32 x * float32 width, float32 y * float32 height, float32 width, float32 height)

        Raylib.DrawRectangleLinesEx(rectangle, thickness, color)

    let cell x y width height color =
        Raylib.DrawRectangle(x * width, y * height, width, height, Color.Black)
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
                buttonBorderRectangle button.X button.Y button.Size,
                calculateBorderSize button.Size,
                Color.Black
            )

    let textBox x y fontSize (text: string) =
        Raylib.DrawText(text, x, y, fontSize, Color.Black)
