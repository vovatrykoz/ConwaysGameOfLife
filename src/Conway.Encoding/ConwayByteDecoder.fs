namespace Conway.Encoding

open Conway.Core
open System

type ConwayByteDecoder() =

    static member val BitsInByte = 8 with get

    static member val RowValueFirstByteIndex = 0 with get

    static member val ColumnValueFirstByteIndex = 4 with get

    static member val GenerationCounterFirstByteIndex = 8 with get

    static member val StartingGenerationCounterFirstByteIndex = 12 with get

    static member val GridFirstByteIndex = 16 with get

    member _.DecodeDimensions(bytes: ReadOnlySpan<byte>) =
        let rowSlice = bytes.Slice(ConwayByteDecoder.RowValueFirstByteIndex, sizeof<int32>)

        let colSlice =
            bytes.Slice(ConwayByteDecoder.ColumnValueFirstByteIndex, sizeof<int32>)

        let rows = BitConverter.ToInt32 rowSlice
        let cols = BitConverter.ToInt32 colSlice

        rows, cols

    member _.DecodeGeneration(bytes: ReadOnlySpan<byte>) =
        let generationCounterSlice =
            bytes.Slice(ConwayByteDecoder.GenerationCounterFirstByteIndex, sizeof<int32>)

        let startingGenerationCounterSlice =
            bytes.Slice(ConwayByteDecoder.StartingGenerationCounterFirstByteIndex, sizeof<int32>)

        BitConverter.ToInt32 generationCounterSlice, BitConverter.ToInt32 startingGenerationCounterSlice

    member private _.DecodeGridSlice
        (bytes: ReadOnlySpan<byte>, startIndex: int, rows: int, cols: int, totalCells: int, totalBytes: int)
        =

        let grid = Array2D.create rows cols ConwayGrid.DeadCell
        let mutable remaining = totalCells

        bytes.Slice(startIndex, totalBytes).ToArray()
        |> Array.map BitVector8.createFromByte
        |> Array.iteri (fun i bits ->
            for j in 0 .. ConwayByteDecoder.BitsInByte - 1 do
                if remaining > 0 then
                    let idx = i * ConwayByteDecoder.BitsInByte + j
                    let r = idx / cols
                    let c = idx % cols

                    grid.[r, c] <-
                        if bits.ReadBitAt j then
                            ConwayGrid.LivingCell
                        else
                            ConwayGrid.DeadCell

                    remaining <- remaining - 1)

        grid

    member this.DecodeGrid(bytes: ReadOnlySpan<byte>) =
        let rows, cols = this.DecodeDimensions bytes
        let totalCells = rows * cols

        let totalBytes =
            (totalCells + ConwayByteDecoder.BitsInByte - 1) / ConwayByteDecoder.BitsInByte

        let currentGrid =
            this.DecodeGridSlice(bytes, ConwayByteDecoder.GridFirstByteIndex, rows, cols, totalCells, totalBytes)

        let startGrid =
            this.DecodeGridSlice(
                bytes,
                ConwayByteDecoder.GridFirstByteIndex + totalBytes,
                rows,
                cols,
                totalCells,
                totalBytes
            )

        let current i j = currentGrid.[i, j]

        ConwayGrid.init cols rows current, startGrid

    interface IConwayByteDecoder with
        member this.Decode(bytes: byte array) =
            let grid, initGrid = this.DecodeGrid(ReadOnlySpan bytes)
            let generation, startingGeneration = this.DecodeGeneration(ReadOnlySpan bytes)

            Game.createFrom (grid, initGrid, generation, startingGeneration)
