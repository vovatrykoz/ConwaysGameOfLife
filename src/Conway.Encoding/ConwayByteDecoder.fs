namespace Conway.Encoding

open Conway.Core
open System
open System.Collections.Generic

type ConwayByteDecoder() =
    static member val NumberOfBytesInInt32 = 4 with get

    static member val RowValueFirstByteIndex = 0 with get

    static member val ColumnValueFirstByteIndex = 4 with get

    static member val GridFirstByteIndex = 8 with get

    member _.DecodeDimensions(bytes: byte array) =
        let rowSlice =
            ReadOnlySpan(bytes).Slice(ConwayByteDecoder.RowValueFirstByteIndex, ConwayByteDecoder.NumberOfBytesInInt32)

        let colSlice =
            ReadOnlySpan(bytes)
                .Slice(ConwayByteDecoder.ColumnValueFirstByteIndex, ConwayByteDecoder.NumberOfBytesInInt32)

        let rows = BitConverter.ToInt32 rowSlice
        let cols = BitConverter.ToInt32 colSlice

        rows, cols

    member _.DecodeGrid(bytes: byte array) = failwith "Not implemented"

    interface IConwayByteDecoder with
        member this.Decode(bytes: byte array) = failwith "Not implemented"
