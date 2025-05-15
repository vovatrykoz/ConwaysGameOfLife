namespace Conway.App.Raylib.File

open Conway.App.Raylib

type ICanvasFileSaver =
    abstract member Save: canvas: Canvas -> path: string -> Result<Option<string>, string>
