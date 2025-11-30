namespace Conway.Tests

open Conway.Core
open NUnit.Framework
open FsCheck
open FsCheck.NUnit

[<Properties(Arbitrary = [| typeof<ConwayGen> |])>]
module ``Grid Properties`` =
    //
    module ``Creation Tests`` =
        //
        [<Property>]
        let ``A dead grid creation method always results in a dead grid being created (including borders)``
            (width: PositiveInt, height: PositiveInt)
            =
            let deadGrid = ConwayGrid.createDead width.Get height.Get

            Assert.That(deadGrid.Board, Is.All.EqualTo ConwayGrid.DeadCell)

        [<Property>]
        let ``A living grid creation method always results in a dead borders``
            (width: PositiveInt, height: PositiveInt)
            =
            let livingGrid = ConwayGrid.createLiving width.Get height.Get

            let borderValues = [|
                let width = livingGrid.Board |> Array2D.length2
                let height = livingGrid.Board |> Array2D.length1

                for i in 0 .. (height - 1) do
                    yield livingGrid.Board.[i, 0]
                    yield livingGrid.Board.[i, width - 1]

                for j in 0 .. (width - 1) do
                    yield livingGrid.Board.[0, j]
                    yield livingGrid.Board.[height - 1, j]
            |]

            Assert.That(borderValues, Is.All.EqualTo ConwayGrid.DeadCell)

        [<Property>]
        let ``A living grid creation method always results in a living grid (except borders)``
            (width: PositiveInt, height: PositiveInt)
            =
            let livingGrid = ConwayGrid.createLiving width.Get height.Get

            let actualPlayableArea = [|
                let width = livingGrid.Board |> Array2D.length2
                let height = livingGrid.Board |> Array2D.length1

                for i in 1 .. (height - 2) do
                    for j in 1 .. (width - 2) do
                        yield livingGrid.Board.[i, j]
            |]

            Assert.That(actualPlayableArea, Is.All.EqualTo ConwayGrid.LivingCell)

        [<Property>]
        let ``The copy method on the grid always results in a new object being created`` (original: ConwayGrid) =
            let copy = ConwayGrid.copyFrom original

            Assert.That(copy, Is.Not.EqualTo original)

        [<Property>]
        let ``The copy method on the grid always produces a copy that has the exact same values as the original``
            (original: ConwayGrid)
            =
            let copied = ConwayGrid.copyFrom original

            Assert.That(copied.Board, Is.EqualTo original.Board)

        [<Property>]
        let ``"Game of Life" rules always hold`` (generatedBoard: Valid2dBoard) =
            let board = generatedBoard.Get
            let l1, l2 = board |> Array2D.length1, board |> Array2D.length2

            let boardWithBorders =
                Array2D.init (l1 + 2) (l2 + 2) (fun i j ->
                    if i = 0 || i = l1 + 1 || j = 0 || j = l2 + 1 then
                        ConwayGrid.DeadCell
                    else
                        board.[i - 1, j - 1])

            let expectedBoard =
                let l1, l2 =
                    boardWithBorders |> Array2D.length1, boardWithBorders |> Array2D.length2

                Array2D.init l1 l2 (fun i j ->
                    if i = 0 || i = l1 - 1 || j = 0 || j = l2 - 1 then
                        ConwayGrid.DeadCell
                    else
                        let ul = boardWithBorders.[i - 1, j - 1]
                        let us = boardWithBorders.[i - 1, j]
                        let ur = boardWithBorders.[i - 1, j + 1]
                        let l = boardWithBorders.[i, j - 1]
                        let r = boardWithBorders.[i, j + 1]
                        let dl = boardWithBorders.[i + 1, j - 1]
                        let ds = boardWithBorders.[i + 1, j]
                        let dr = boardWithBorders.[i + 1, j + 1]

                        let sum = ul + us + ur + l + r + dl + ds + dr

                        match sum with
                        | 2<CellStatus> -> boardWithBorders.[i, j]
                        | 3<CellStatus> -> ConwayGrid.LivingCell
                        | _ -> ConwayGrid.DeadCell)

            let width = board |> Array2D.length2
            let height = board |> Array2D.length1
            let initFunc i j = board.[i, j]
            let actual = ConwayGrid.init width height initFunc
            actual.AdvanceToNextState()

            Assert.That(actual.Board, Is.EqualTo expectedBoard)
