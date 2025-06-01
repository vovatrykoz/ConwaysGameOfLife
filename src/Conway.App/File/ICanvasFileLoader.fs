namespace Conway.App.File

open Conway.App
open Conway.Core

[<NoComparison>]
type CanvasWrapper = {
    Game: Game
    Camera: Camera
    OptionalMessage: Option<string>
}

[<Interface>]
type ICanvasFileLoader =
    abstract member Load: path: string -> Result<CanvasWrapper, string>
