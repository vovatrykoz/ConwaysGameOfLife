namespace Conway.Core

[<Interface>]
type IRenderer =
    abstract member RenderGrid: ConwayGrid -> unit
