namespace Conway.Tests

open Conway.Core
open FsCheck.FSharp

type Valid2dBoard(board: int<CellStatus> array2d) =
    member _.Get = board

module internal Generators =
    //
    module ConwayGrid =
        let validConwayGridGen () =
            ArbMap.defaults.ArbFor<int<CellStatus> array2d>()
            |> Arb.toGen
            |> Gen.filter (fun xs ->
                xs
                |> Seq.cast<int<CellStatus>>
                |> Seq.forall (fun x -> x = ConwayGrid.DeadCell || x = ConwayGrid.LivingCell))

        let validConwayGridArb () =
            validConwayGridGen ()
            |> Gen.map (fun xs ->
                let width, height = Array2D.length2 xs, Array2D.length1 xs
                let initFunc i j = xs.[i, j]
                ConwayGrid.init width height initFunc)
            |> Arb.fromGen

        let valid2dBoardArb () = validConwayGridGen () |> Arb.fromGen

    module Game =
        let validGameArb () =
            gen {
                let! currentGridGen = ConwayGrid.validConwayGridArb () |> Arb.toGen
                let! initialGridGen = ConwayGrid.validConwayGridArb () |> Arb.toGen
                let! generationCounter = ArbMap.defaults.ArbFor<int>() |> Arb.toGen

                return currentGridGen, initialGridGen, generationCounter
            }
            |> Gen.map (fun (cg, ig, gc) -> Game.createFrom cg ig gc)
            |> Arb.fromGen

type ConwayGen =
    //
    static member ConwayGrid() =
        Generators.ConwayGrid.validConwayGridArb ()

    static member Valid2dBoard() =
        Generators.ConwayGrid.valid2dBoardArb ()

    static member Game() = Generators.Game.validGameArb ()
