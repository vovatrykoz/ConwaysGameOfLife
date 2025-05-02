module Conway.Tests

open Conway.Core
open NUnit.Framework

module ``Grid tests`` =

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

    [<Test>]
    let ``Can create a living grid using the dedicated method on the Grid type`` () =
        let expectedArray = [|
            [| 0<CellStatus>; 0<CellStatus>; 0<CellStatus>; 0<CellStatus> |]
            [| 0<CellStatus>; 1<CellStatus>; 1<CellStatus>; 0<CellStatus> |]
            [| 0<CellStatus>; 1<CellStatus>; 1<CellStatus>; 0<CellStatus> |]
            [| 0<CellStatus>; 0<CellStatus>; 0<CellStatus>; 0<CellStatus> |]
        |]

        let expectedBoard = Array2D.init 4 4 (fun i j -> expectedArray[i][j])

        let actual = ConwayGrid.createLiving 2 2

        Assert.That(actual.Board, Is.EqualTo expectedBoard)

    [<Test>]
    let ``Can create a living grid using the init method on the Grid type`` () =
        let expectedArray = [|
            [| 0<CellStatus>; 0<CellStatus>; 0<CellStatus>; 0<CellStatus>; 0<CellStatus> |]
            [| 0<CellStatus>; 1<CellStatus>; 0<CellStatus>; 1<CellStatus>; 0<CellStatus> |]
            [| 0<CellStatus>; 0<CellStatus>; 1<CellStatus>; 0<CellStatus>; 0<CellStatus> |]
            [| 0<CellStatus>; 1<CellStatus>; 0<CellStatus>; 1<CellStatus>; 0<CellStatus> |]
            [| 0<CellStatus>; 0<CellStatus>; 0<CellStatus>; 0<CellStatus>; 0<CellStatus> |]
        |]

        let initializer i j =
            if (i + j) % 2 = 0 then 1<CellStatus> else 0<CellStatus>

        let expectedBoard = Array2D.init 5 5 (fun i j -> expectedArray[i][j])

        let actual = ConwayGrid.init 3 3 initializer

        Assert.That(actual.Board, Is.EqualTo expectedBoard)

    [<Test>]
    let ``All-dead grid remains0<CellStatus> after one iteration`` () =
        let initializer _ _ = 0<CellStatus>

        let expectedBoard = (ConwayGrid.init 3 3 initializer).Board

        let actual = ConwayGrid.createDead 3 3
        actual.AdvanceToNextState()

        Assert.That(actual.Board, Is.EqualTo expectedBoard)

    [<Test>]
    let ``A cell dies with no living neighbors`` () =
        let initializerForExpected i j =
            if i = 1 && j = 1 then 0<CellStatus> else 0<CellStatus>

        let initializerForActual i j =
            if i = 1 && j = 1 then 1<CellStatus> else 0<CellStatus>

        let expectedBoard = (ConwayGrid.init 3 3 initializerForExpected).Board

        let actual = ConwayGrid.init 3 3 initializerForActual
        actual.AdvanceToNextState()

        Assert.That(actual.Board, Is.EqualTo expectedBoard)

    [<Test>]
    let ``A cell dies with one living neighbors`` () =
        let initializerForExpected i j =
            if i = 1 && j = 1 || i = 2 && j = 2 then
                0<CellStatus>
            else
                0<CellStatus>

        let initializerForActual i j =
            if i = 1 && j = 1 || i = 2 && j = 2 then
                1<CellStatus>
            else
                0<CellStatus>

        let expectedBoard = (ConwayGrid.init 3 3 initializerForExpected).Board

        let actual = ConwayGrid.init 3 3 initializerForActual
        actual.AdvanceToNextState()

        Assert.That(actual.Board, Is.EqualTo expectedBoard)

    [<Test>]
    let ``A cell becomes1<CellStatus> with three living neighbors`` () =
        let setup = [| [| 1<CellStatus>; 1<CellStatus> |]; [| 1<CellStatus>; 0<CellStatus> |] |]

        let expectedArray = [| [| 1<CellStatus>; 1<CellStatus> |]; [| 1<CellStatus>; 1<CellStatus> |] |]

        let setupInitializer i j = setup[i][j]
        let expectedInitializer i j = expectedArray[i][j]

        let actual = ConwayGrid.init 2 2 setupInitializer
        actual.AdvanceToNextState()

        let expectedBoard = (ConwayGrid.init 2 2 expectedInitializer).Board

        Assert.That(actual.Board, Is.EqualTo expectedBoard)

    [<Test>]
    let ``A living cell with three living neighbors keeps on living`` () =
        // each individual cell has three living neighbors
        let setup = [| [| 1<CellStatus>; 1<CellStatus> |]; [| 1<CellStatus>; 1<CellStatus> |] |]

        let expectedArray = [| [| 1<CellStatus>; 1<CellStatus> |]; [| 1<CellStatus>; 1<CellStatus> |] |]

        let setupInitializer i j = setup[i][j]
        let expectedInitializer i j = expectedArray[i][j]

        let actual = ConwayGrid.init 2 2 setupInitializer
        actual.AdvanceToNextState()

        let expectedBoard = (ConwayGrid.init 2 2 expectedInitializer).Board

        Assert.That(actual.Board, Is.EqualTo expectedBoard)

    [<Test>]
    let ``A cell with four living neighbors dies`` () =
        let setup = [|
            [| 1<CellStatus>; 1<CellStatus>; 1<CellStatus> |]
            [| 1<CellStatus>; 1<CellStatus>; 0<CellStatus> |]
        |]

        // the two middle cells both have 4 living neighbors, therefore both are expected to die
        // The cell in the bottom right should become1<CellStatus>, as it had three living neighbors
        let expectedArray = [|
            [| 1<CellStatus>; 0<CellStatus>; 1<CellStatus> |]
            [| 1<CellStatus>; 0<CellStatus>; 1<CellStatus> |]
        |]

        let setupInitializer i j = setup[i][j]
        let expectedInitializer i j = expectedArray[i][j]

        let actual = ConwayGrid.init 3 2 setupInitializer
        actual.AdvanceToNextState()

        let expectedBoard = (ConwayGrid.init 3 2 expectedInitializer).Board

        Assert.That(actual.Board, Is.EqualTo expectedBoard)

    [<Test>]
    let ``Three cells in a row stay1<CellStatus> by switching to rows and columns`` () =
        let setup = [|
            [| 0<CellStatus>; 0<CellStatus>; 0<CellStatus> |]
            [| 1<CellStatus>; 1<CellStatus>; 1<CellStatus> |]
            [| 0<CellStatus>; 0<CellStatus>; 0<CellStatus> |]
        |]

        let ``expected array after the first iteration`` = [|
            [| 0<CellStatus>; 1<CellStatus>; 0<CellStatus> |]
            [| 0<CellStatus>; 1<CellStatus>; 0<CellStatus> |]
            [| 0<CellStatus>; 1<CellStatus>; 0<CellStatus> |]
        |]

        let ``expected array after the second iteration`` = [|
            [| 0<CellStatus>; 0<CellStatus>; 0<CellStatus> |]
            [| 1<CellStatus>; 1<CellStatus>; 1<CellStatus> |]
            [| 0<CellStatus>; 0<CellStatus>; 0<CellStatus> |]
        |]

        let setupInitializer i j = setup[i][j]

        let expectedInitializerOne i j =
            ``expected array after the first iteration``[i][j]

        let expectedInitializerTwo i j =
            ``expected array after the second iteration``[i][j]

        let actualOne = ConwayGrid.init 3 3 setupInitializer
        actualOne.AdvanceToNextState()

        let actualBoardOne = actualOne.Board

        let expectedBoardOne = (ConwayGrid.init 3 3 expectedInitializerOne).Board

        Assert.That(actualBoardOne, Is.EqualTo expectedBoardOne)

        actualOne.AdvanceToNextState()
        let actualBoardTwo = actualOne.Board

        let expectedBoardTwo = (ConwayGrid.init 3 3 expectedInitializerTwo).Board

        Assert.That(actualBoardTwo, Is.EqualTo expectedBoardTwo)
