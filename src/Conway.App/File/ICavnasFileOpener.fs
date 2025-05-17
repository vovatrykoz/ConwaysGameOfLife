namespace Conway.App.File

open Conway.App

[<NoComparison>]
type CanvasWrapper = {
    Game: Canvas
    OptionalMessage: Option<string>
}

[<Interface>]
type ICanvasFileOpener =
    abstract member Open: path: string -> Result<CanvasWrapper, string>
