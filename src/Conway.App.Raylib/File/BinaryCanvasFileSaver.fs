namespace Conway.App.Raylib.File

open Conway.App.Raylib
open Conway.Core
open System
open System.Collections.Generic

type BinaryCanvasFileSaver() =

    member _.Encode(grid: ConwayGrid) =
        let rows = Array2D.length1 grid.Board - 2
        let cols = Array2D.length2 grid.Board - 2

        let dimensionsEncoded =
            Array.append (BitConverter.GetBytes rows) (BitConverter.GetBytes cols)

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
                | 0<CellStatus> -> ()
                | _ -> gridBytes.[currentIndex] <- currentBitVector.SetBitAt bitCounter

                bitCounter <- bitCounter + 1

        gridBytes.ToArray()
        |> Array.map (fun bitVector -> bitVector.Byte)
        |> Array.append dimensionsEncoded

    member this.Encode(game: Game) = failwith "Not Implemented"

    interface ICanvasFileSaver with
        member this.Save (canvas: Canvas) (path: string) : Result<Option<string>, string> = failwith "Not Implemented"
