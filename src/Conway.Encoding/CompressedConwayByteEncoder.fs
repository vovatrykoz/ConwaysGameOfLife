namespace Conway.Encoding

open Conway.Core
open System
open System.Collections.Generic

type CompressedConwayByteEncoder(compressor: IByteCompressor) =

    member _.Compressor = compressor

    member _.EncodeDimensions(grid: ConwayGrid) =
        let rows = Array2D.length1 grid.Board - 2
        let cols = Array2D.length2 grid.Board - 2

        Array.append (BitConverter.GetBytes rows) (BitConverter.GetBytes cols)

    member _.EncodeGrid(grid: ConwayGrid) =
        let rows = Array2D.length1 grid.Board - 2
        let cols = Array2D.length2 grid.Board - 2

        let gridBytes = List<BitVector8>()
        gridBytes.Add BitVector8.zeroed

        let mutable bitCounter = 0

        for row in 1..rows do
            for col in 1..cols do
                if bitCounter >= 8 then
                    gridBytes.Add BitVector8.zeroed
                    bitCounter <- 0

                let currentIndex = gridBytes.Count - 1
                let currentBitVector = gridBytes.[currentIndex]

                match grid.Board.[row, col] with
                | x when x = ConwayGrid.DeadCell -> ()
                | _ -> gridBytes.[currentIndex] <- currentBitVector.SetBitAt bitCounter

                bitCounter <- bitCounter + 1

        gridBytes.ToArray() |> Array.map (fun bitVector -> bitVector.Byte)

    interface IConwayByteEncoder with
        member this.Encode(game: Game) =
            let dimensionsEncoded = this.EncodeDimensions game.CurrentState
            let currentGridEncoded = this.EncodeGrid game.CurrentState
            let initialGridEncoded = this.EncodeGrid game.InitialState
            let generationEncoded = BitConverter.GetBytes game.Generation

            initialGridEncoded
            |> Array.append currentGridEncoded
            |> Array.append generationEncoded
            |> Array.append dimensionsEncoded
            |> this.Compressor.Compress
