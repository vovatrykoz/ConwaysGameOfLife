namespace Conway.Encoding

open Conway.Core

[<Interface>]
type IConwayByteDecoder =
    abstract member Decode: bytes: byte array -> Game
