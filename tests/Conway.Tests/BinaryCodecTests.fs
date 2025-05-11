namespace Conway.Tests

open Conway.Core
open NUnit.Framework

module ``Binary Codec Tests`` =

    [<Test>]
    let ``Can create a simple grid using the dedicated method on the Grid type`` () =
        let expectedArray = [|
            [| 0<CellStatus>; 0<CellStatus>; 0<CellStatus>; 0<CellStatus> |]
            [| 0<CellStatus>; 0<CellStatus>; 0<CellStatus>; 0<CellStatus> |]
            [| 0<CellStatus>; 0<CellStatus>; 0<CellStatus>; 0<CellStatus> |]
            [| 0<CellStatus>; 0<CellStatus>; 0<CellStatus>; 0<CellStatus> |]
        |]

        let expectedBoard = Array2D.init 4 4 (fun i j -> expectedArray[i][j])

        let actual = ConwayGrid.createDead 2 2

        Assert.That(actual.Board, Is.EqualTo expectedBoard)
