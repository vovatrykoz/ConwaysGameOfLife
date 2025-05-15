namespace Conway.Encoding

open Conway.Core

[<Interface>]
type IConwayByteEncoder =
    abstract member Encode: game: Game -> byte array
