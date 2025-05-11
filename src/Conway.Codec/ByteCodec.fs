namespace Conway.Codec

type ByteCodec() =
    interface IConwayCodec with
        member this.Decode(game: byte array) : Conway.Core.Game = failwith "Not Implemented"
        member this.Encode(game: Conway.Core.Game) : byte array = failwith "Not Implemented"
