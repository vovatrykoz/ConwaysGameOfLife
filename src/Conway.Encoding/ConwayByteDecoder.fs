namespace Conway.Encoding

open Conway.Core
open System
open System.Collections.Generic

type ConwayByteDecoder() =

    static member val BitsInByte = 8 with get

    static member val RowValueFirstByteIndex = 0 with get

    static member val ColumnValueFirstByteIndex = 4 with get

    static member val GridFirstByteIndex = 12 with get

    member _.DecodeDimensions(bytes: ReadOnlySpan<byte>) =
        let rowSlice = bytes.Slice(ConwayByteDecoder.RowValueFirstByteIndex, sizeof<int32>)

        let colSlice =
            bytes.Slice(ConwayByteDecoder.ColumnValueFirstByteIndex, sizeof<int32>)

        let rows = BitConverter.ToInt32 rowSlice
        let cols = BitConverter.ToInt32 colSlice

        rows, cols

    member this.DecodeGrid(bytes: ReadOnlySpan<byte>) =
        let rows, cols = this.DecodeDimensions bytes
        let totalCells = rows * cols
        let totalBytes = totalCells / ConwayByteDecoder.BitsInByte
        let cellArray = Array2D.create rows cols 0<CellStatus>

        bytes.Slice(ConwayByteDecoder.GridFirstByteIndex, totalBytes).ToArray()
        |> Array.map (fun b -> BitVector8.createFromByte b)
        |> Array.iteri (fun i b ->
            for j in 0..7 do
                let actualIndex = i * ConwayByteDecoder.BitsInByte + j
                let row = actualIndex / cols
                let col = actualIndex % cols

                match b.ReadBitAt j with
                | true -> cellArray.[row, col] <- 1<CellStatus>
                | false -> cellArray.[row, col] <- 0<CellStatus>)

        let initializer i j = cellArray.[i, j]
        ConwayGrid.init rows cols initializer

    interface IConwayByteDecoder with
        member this.Decode(bytes: byte array) =
            let grid = this.DecodeGrid(ReadOnlySpan bytes)

            Game grid
