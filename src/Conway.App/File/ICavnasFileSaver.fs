namespace Conway.App.File

open Conway.App

[<Interface>]
type ICanvasFileSaver =
    abstract member Save: canvas: Canvas -> path: string -> Result<Option<string>, string>
