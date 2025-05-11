namespace Conway.Codec

open Conway.Core

type IConwayCodec =
    abstract member Encode: game: Game -> byte[]
    abstract member Decode: game: byte[] -> Game
