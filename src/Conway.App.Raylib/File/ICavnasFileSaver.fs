namespace Conway.App.Raylib.File

open Conway.App.Raylib

[<Interface>]
type ICanvasFileSaver =
    abstract member Save: canvas: Canvas -> path: string -> Result<Option<string>, string>
