namespace Conway.App.Raylib.File

open Conway.App.Raylib

[<NoComparison>]
type CanvasWrapper = {
    Game: Canvas
    OptionalMessage: Option<string>
}

type ICanvasFileOpener =
    abstract member Open: path: string -> Result<CanvasWrapper, string>
