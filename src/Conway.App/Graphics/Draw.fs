namespace Conway.App.Graphics

open Conway.App.Controls
open Raylib_cs

module Draw =
    open System.Numerics

    let inline private buttonBorderRectangle x y size =
        Rectangle(float32 (x - 2), float32 (y - 2), float32 (size + 4), float32 (size + 4))

    let inline private calculateBorderSize buttonSize = float32 buttonSize / 12.5f

    let border x y width height thickness color =
        let rectangle =
            Rectangle(float32 x * float32 width, float32 y * float32 height, float32 width, float32 height)

        Raylib.DrawRectangleLinesEx(rectangle, thickness, color)

    let cell (x: float32) (y: float32) (width: float32) (height: float32) color =
        Raylib.DrawRectanglePro(
            Rectangle(Vector2(x * width, y * height), Vector2(width, height)),
            Vector2.Zero,
            0.0f,
            Color.Black
        )

        Raylib.DrawRectanglePro(
            Rectangle(Vector2(x * width + 1.0f, y * height + 1.0f), Vector2(width - 2.0f, height - 2.0f)),
            Vector2.Zero,
            0.0f,
            color
        )

    let inline livingCell x y width height = cell x y width height Color.Red

    let inline deadCell x y width height = cell x y width height Color.Black

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

    let inline textBox (x: float32) (y: float32) fontSize (text: string) =
        Raylib.DrawText(text, int x, int y, fontSize, Color.Black)

    let listBox (x: float32, y: float32, items: List<string>) =
        Raylib.DrawRectangle(int x, int y, 10, 10, Color.Black)
