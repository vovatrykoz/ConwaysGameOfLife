namespace Conway.Core

[<Interface>]
type IInputReader =
    abstract member ReadGrid: unit -> Grid
