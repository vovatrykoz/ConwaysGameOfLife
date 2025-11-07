namespace Conway.App.File

open Conway.App.Controls
open Conway.App.Math
open Conway.Core

[<NoComparison>]
type CanvasWrapper = {
    Game: Game
    Camera: Camera<cells>
    OptionalMessage: Option<string>
}

[<Interface>]
type ICanvasFileLoader =
    abstract member Load: path: string -> Result<CanvasWrapper, string>
