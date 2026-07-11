namespace Conway.App.Graphics

module Draw =
    open Conway.App.Controls
    open Conway.App.Math
    open Raylib_cs
    open System.Numerics

    let inline private buttonBorderRectangle x y width height =
        Rectangle(float32 (x - 2<px>), float32 (y - 2<px>), float32 (width + 4<px>), float32 (height + 4<px>))

    let inline private calculateBorderSize buttonSize = float32 buttonSize / 12.5f

    let border x y width height thickness color =
        let rectangle =
            Rectangle(float32 x * float32 width, float32 y * float32 height, float32 width, float32 height)

        Raylib.DrawRectangleLinesEx(rectangle, thickness, color)

    let cell (x: float32<px>) (y: float32<px>) (width: float32<px>) (height: float32<px>) color =
        Raylib.DrawRectanglePro(
            Rectangle(Vector2(float32 x, float32 y), Vector2(float32 width, float32 height)),
            Vector2.Zero,
            0.0f,
            Color.Black
        )

        Raylib.DrawRectanglePro(
            Rectangle(Vector2(float32 x + 1.0f, float32 y + 1.0f), Vector2(float32 width - 2.0f, float32 height - 2.0f)),
            Vector2.Zero,
            0.0f,
            color
        )

    let inline livingCell x y width height = cell x y width height Color.Red

    let inline deadCell x y width height = cell x y width height Color.Black

    let button (button: Button) =
        match button.IsActive with
        | true -> Raylib.DrawRectangle(int button.X, int button.Y, int button.Width, int button.Height, Color.Black)
        | false -> Raylib.DrawRectangle(int button.X, int button.Y, int button.Width, int button.Height, Color.Gray)

        Raylib.DrawText(button.Text, int (button.X + 5<px>), int (button.Y + 5<px>), 15, Color.White)

        match button.IsPressed with
        | false -> ()
        | true ->
            Raylib.DrawRectangleLinesEx(
                buttonBorderRectangle button.X button.Y button.Width button.Height,
                calculateBorderSize (max button.Width button.Height),
                Color.Black
            )

    let inline label
        (x: float32<px>)
        (y: float32<px>)
        (fontSize: int)
        (text: string)
        (width: int)
        (height: int)
        (textColor: Color)
        (backgroundColor: Color)
        =
        Raylib.DrawRectangle(int x, int y, int width, int height, backgroundColor)
        Raylib.DrawText(text, int x, int y, fontSize, textColor)

    let inline line
        (startX: float32<px>)
        (startY: float32<px>)
        (endX: float32<px>)
        (endY: float32<px>)
        (thickness: int<px>)
        (color: Color)
        =
        let startPos = Vector2(float32 startX, float32 startY)
        let endPos = Vector2(float32 endX, float32 endY)
        Raylib.DrawLineEx(startPos, endPos, float32 thickness, color)

    let listBox (x: float32<px>, y: float32<px>, items: ResizeArray<string>) =
        Raylib.DrawRectangle(int x, int y, 10, 10, Color.Black)
