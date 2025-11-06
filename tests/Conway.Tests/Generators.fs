namespace Conway.Tests

open Conway.App
open Conway.App.Math
open Conway.Core
open FsCheck.FSharp

type Valid2dBoard(board: int<CellStatus> array2d) =
    member _.Get = board

type ValidWidthValue(value: int) =
    member _.Get = value

type ValidUserInput(value: string) =
    member _.Get = value

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

    module UserInput =
        let positiveValueGen () =
            ArbMap.defaults.ArbFor<int>() |> Arb.toGen |> Gen.filter (fun x -> x > 0)

        let validWidthValueArb () =
            positiveValueGen () |> Gen.map (fun x -> ValidWidthValue x) |> Arb.fromGen

        let validHeightValueArb () =
            positiveValueGen () |> Gen.map (fun x -> ValidWidthValue x) |> Arb.fromGen

        let validWidthSwitchesGen () = Gen.elements [ "-w"; "--width" ]

        let validHeightSwitchesGen () = Gen.elements [ "-h"; "--height" ]

        let spacesGen () =
            Gen.choose (1, 1000) |> Gen.map (fun n -> String.replicate n " ")

        let validUserInputGen () =
            gen {
                let! validWidthSwitch = validWidthSwitchesGen ()
                let! validWidth = positiveValueGen ()

                let! validHeightSwitch = validHeightSwitchesGen ()
                let! validHeight = positiveValueGen ()

                let! s1 = spacesGen ()
                let! s2 = spacesGen ()
                let! s3 = spacesGen ()
                let! s4 = spacesGen ()
                let! s5 = spacesGen ()

                return!
                    Gen.elements [
                        $"{s1}{validWidthSwitch}{s2}{validWidth}{s3}{validHeightSwitch}{s4}{validHeight}{s5}"
                        $"{s1}{validHeightSwitch}{s2}{validHeight}{s3}{validWidthSwitch}{s4}{validWidth}{s5}"
                    ]
            }

        let validUserInputArb () =
            validUserInputGen () |> Gen.map (fun x -> ValidUserInput x) |> Arb.fromGen

    module Application =
        open Conway.App.Controls

        let cameraGen () =
            ArbMap.defaults.ArbFor<float32<cells>>()
            |> Arb.toGen
            |> Gen.three
            |> Gen.map (fun (x, y, zoomFactor) -> Camera(x, y, float32 zoomFactor))

        let cameraArb () = cameraGen () |> Arb.fromGen

        let floatGen () =
            ArbMap.defaults.ArbFor<float32>() |> Arb.toGen

        let cellCoordGen () =
            ArbMap.defaults.ArbFor<float32<cells>>() |> Arb.toGen

        let pixelCoordGen () =
            ArbMap.defaults.ArbFor<float32<px>>() |> Arb.toGen

        let canvasGen () =
            gen {
                let! x = pixelCoordGen ()
                let! y = pixelCoordGen ()
                let! width = pixelCoordGen ()
                let! height = pixelCoordGen ()
                let! camera = cameraGen ()
                let! game = Game.validGameArb () |> Arb.toGen
                let! cellSize = pixelCoordGen ()

                return Canvas(x, y, width, height, camera, game, cellSize)
            }

        let canvasArb () = canvasGen () |> Arb.fromGen

    module Math =
        open System
        open System.Numerics

        let vector2Gen () =
            ArbMap.defaults.ArbFor<float32>()
            |> Arb.toGen
            |> Gen.two
            |> Gen.map (fun (x, y) -> Vector2(x, y))

        let vector2Arb () = vector2Gen () |> Arb.fromGen

type ConwayGen =
    //
    static member ConwayGrid() =
        Generators.ConwayGrid.validConwayGridArb ()

    static member Valid2dBoard() =
        Generators.ConwayGrid.valid2dBoardArb ()

    static member Game() = Generators.Game.validGameArb ()

    static member ValidUserInput() =
        Generators.UserInput.validUserInputArb ()

    static member Camera() = Generators.Application.cameraArb ()

    static member Canvas() = Generators.Application.canvasArb ()

    static member Vector2() = Generators.Math.vector2Arb ()
