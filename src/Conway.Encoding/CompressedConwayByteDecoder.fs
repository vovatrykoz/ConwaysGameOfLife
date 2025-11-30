namespace Conway.Encoding

open Conway.Core
open System

type CompressedConwayByteDecoder(decompressor: IByteDecompressor) =

    static member val BitsInByte = 8 with get

    static member val RowValueFirstByteIndex = 0 with get

    static member val ColumnValueFirstByteIndex = 4 with get

    static member val GenerationCounterFirstByteIndex = 8 with get

    static member val GridFirstByteIndex = 12 with get

    member _.Decompressor = decompressor

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

        BitConverter.ToInt32 generationCounterSlice

    member this.DecodeGrid(bytes: ReadOnlySpan<byte>) =
        let rows, cols = this.DecodeDimensions bytes
        let totalCells = rows * cols
        let totalBytes = totalCells / ConwayByteDecoder.BitsInByte + 1
        let cellArray = Array2D.create rows cols ConwayGrid.DeadCell

        let mutable remainingCells = totalCells

        bytes.Slice(ConwayByteDecoder.GridFirstByteIndex, totalBytes).ToArray()
        |> Array.map (fun b -> BitVector8.createFromByte b)
        |> Array.iteri (fun i b ->
            for j in 0..7 do
                if remainingCells <= 0 then
                    ()
                else
                    let actualIndex = i * ConwayByteDecoder.BitsInByte + j
                    let row = actualIndex / cols
                    let col = actualIndex % cols

                    match b.ReadBitAt j with
                    | true -> cellArray.[row, col] <- ConwayGrid.LivingCell
                    | false -> cellArray.[row, col] <- ConwayGrid.DeadCell

                    remainingCells <- remainingCells - 1)

        let initializer i j = cellArray.[i, j]
        ConwayGrid.init cols rows initializer

    interface IConwayByteDecoder with
        member this.Decode(bytes: byte array) =
            let decompressed = this.Decompressor.Decompress bytes
            let grid = this.DecodeGrid(ReadOnlySpan decompressed)
            let generation = this.DecodeGeneration(ReadOnlySpan decompressed)
            new Game(grid, generation)
