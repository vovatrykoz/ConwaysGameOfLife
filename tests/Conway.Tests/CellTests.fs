namespace Conway.Tests

open Conway.Core
open NUnit.Framework

module ``Grid tests`` =

    [<Test>]
    let ``Can create a simple grid using the dedicated method on the Grid type`` () =
        let expectedArray = [|
            [| 0uy<CellStatus>; 0uy<CellStatus>; 0uy<CellStatus>; 0uy<CellStatus> |]
            [| 0uy<CellStatus>; 0uy<CellStatus>; 0uy<CellStatus>; 0uy<CellStatus> |]
            [| 0uy<CellStatus>; 0uy<CellStatus>; 0uy<CellStatus>; 0uy<CellStatus> |]
            [| 0uy<CellStatus>; 0uy<CellStatus>; 0uy<CellStatus>; 0uy<CellStatus> |]
        |]

        let expectedBoard = Array2D.init 4 4 (fun i j -> expectedArray[i][j])

        let actual = ConwayGrid.createDead 2 2

        Assert.That(actual.Board, Is.EqualTo expectedBoard)

    [<Test>]
    let ``Can create a living grid using the dedicated method on the Grid type`` () =
        let expectedArray = [|
            [| 0uy<CellStatus>; 0uy<CellStatus>; 0uy<CellStatus>; 0uy<CellStatus> |]
            [| 0uy<CellStatus>; 1uy<CellStatus>; 1uy<CellStatus>; 0uy<CellStatus> |]
            [| 0uy<CellStatus>; 1uy<CellStatus>; 1uy<CellStatus>; 0uy<CellStatus> |]
            [| 0uy<CellStatus>; 0uy<CellStatus>; 0uy<CellStatus>; 0uy<CellStatus> |]
        |]

        let expectedBoard = Array2D.init 4 4 (fun i j -> expectedArray[i][j])

        let actual = ConwayGrid.createLiving 2 2

        Assert.That(actual.Board, Is.EqualTo expectedBoard)

    [<Test>]
    let ``Can create a living grid using the init method on the Grid type`` () =
        let expectedArray = [|
            [|
                0uy<CellStatus>
                0uy<CellStatus>
                0uy<CellStatus>
                0uy<CellStatus>
                0uy<CellStatus>
            |]
            [|
                0uy<CellStatus>
                1uy<CellStatus>
                0uy<CellStatus>
                1uy<CellStatus>
                0uy<CellStatus>
            |]
            [|
                0uy<CellStatus>
                0uy<CellStatus>
                1uy<CellStatus>
                0uy<CellStatus>
                0uy<CellStatus>
            |]
            [|
                0uy<CellStatus>
                1uy<CellStatus>
                0uy<CellStatus>
                1uy<CellStatus>
                0uy<CellStatus>
            |]
            [|
                0uy<CellStatus>
                0uy<CellStatus>
                0uy<CellStatus>
                0uy<CellStatus>
                0uy<CellStatus>
            |]
        |]

        let initializer i j =
            if (i + j) % 2 = 0 then 1uy<CellStatus> else 0uy<CellStatus>

        let expectedBoard = Array2D.init 5 5 (fun i j -> expectedArray[i][j])

        let actual = ConwayGrid.init 3 3 initializer

        Assert.That(actual.Board, Is.EqualTo expectedBoard)

    [<Test>]
    let ``All-dead grid remains0uy<CellStatus> after one iteration`` () =
        let initializer _ _ = 0uy<CellStatus>

        let expectedBoard = (ConwayGrid.init 3 3 initializer).Board

        let actual = ConwayGrid.createDead 3 3
        actual.AdvanceToNextState()

        Assert.That(actual.Board, Is.EqualTo expectedBoard)

    [<Test>]
    let ``A cell dies with no living neighbors`` () =
        let initializerForExpected i j =
            if i = 1 && j = 1 then 0uy<CellStatus> else 0uy<CellStatus>

        let initializerForActual i j =
            if i = 1 && j = 1 then 1uy<CellStatus> else 0uy<CellStatus>

        let expectedBoard = (ConwayGrid.init 3 3 initializerForExpected).Board

        let actual = ConwayGrid.init 3 3 initializerForActual
        actual.AdvanceToNextState()

        Assert.That(actual.Board, Is.EqualTo expectedBoard)

    [<Test>]
    let ``A cell dies with one living neighbors`` () =
        let initializerForExpected i j =
            if i = 1 && j = 1 || i = 2 && j = 2 then
                0uy<CellStatus>
            else
                0uy<CellStatus>

        let initializerForActual i j =
            if i = 1 && j = 1 || i = 2 && j = 2 then
                1uy<CellStatus>
            else
                0uy<CellStatus>

        let expectedBoard = (ConwayGrid.init 3 3 initializerForExpected).Board

        let actual = ConwayGrid.init 3 3 initializerForActual
        actual.AdvanceToNextState()

        Assert.That(actual.Board, Is.EqualTo expectedBoard)

    [<Test>]
    let ``A cell becomes1uy<CellStatus> with three living neighbors`` () =
        let setup = [|
            [| 1uy<CellStatus>; 1uy<CellStatus> |]
            [| 1uy<CellStatus>; 0uy<CellStatus> |]
        |]

        let expectedArray = [|
            [| 1uy<CellStatus>; 1uy<CellStatus> |]
            [| 1uy<CellStatus>; 1uy<CellStatus> |]
        |]

        let setupInitializer i j = setup[i][j]
        let expectedInitializer i j = expectedArray[i][j]

        let actual = ConwayGrid.init 2 2 setupInitializer
        actual.AdvanceToNextState()

        let expectedBoard = (ConwayGrid.init 2 2 expectedInitializer).Board

        Assert.That(actual.Board, Is.EqualTo expectedBoard)

    [<Test>]
    let ``A living cell with three living neighbors keeps on living`` () =
        // each individual cell has three living neighbors
        let setup = [|
            [| 1uy<CellStatus>; 1uy<CellStatus> |]
            [| 1uy<CellStatus>; 1uy<CellStatus> |]
        |]

        let expectedArray = [|
            [| 1uy<CellStatus>; 1uy<CellStatus> |]
            [| 1uy<CellStatus>; 1uy<CellStatus> |]
        |]

        let setupInitializer i j = setup[i][j]
        let expectedInitializer i j = expectedArray[i][j]

        let actual = ConwayGrid.init 2 2 setupInitializer
        actual.AdvanceToNextState()

        let expectedBoard = (ConwayGrid.init 2 2 expectedInitializer).Board

        Assert.That(actual.Board, Is.EqualTo expectedBoard)

    [<Test>]
    let ``A cell with four living neighbors dies`` () =
        let setup = [|
            [| 1uy<CellStatus>; 1uy<CellStatus>; 1uy<CellStatus> |]
            [| 1uy<CellStatus>; 1uy<CellStatus>; 0uy<CellStatus> |]
        |]

        // the two middle cells both have 4 living neighbors, therefore both are expected to die
        // The cell in the bottom right should become1uy<CellStatus>, as it had three living neighbors
        let expectedArray = [|
            [| 1uy<CellStatus>; 0uy<CellStatus>; 1uy<CellStatus> |]
            [| 1uy<CellStatus>; 0uy<CellStatus>; 1uy<CellStatus> |]
        |]

        let setupInitializer i j = setup[i][j]
        let expectedInitializer i j = expectedArray[i][j]

        let actual = ConwayGrid.init 3 2 setupInitializer
        actual.AdvanceToNextState()

        let expectedBoard = (ConwayGrid.init 3 2 expectedInitializer).Board

        Assert.That(actual.Board, Is.EqualTo expectedBoard)

    [<Test>]
    let ``Three cells in a row stay1uy<CellStatus> by switching to rows and columns`` () =
        let setup = [|
            [| 0uy<CellStatus>; 0uy<CellStatus>; 0uy<CellStatus> |]
            [| 1uy<CellStatus>; 1uy<CellStatus>; 1uy<CellStatus> |]
            [| 0uy<CellStatus>; 0uy<CellStatus>; 0uy<CellStatus> |]
        |]

        let ``expected array after the first iteration`` = [|
            [| 0uy<CellStatus>; 1uy<CellStatus>; 0uy<CellStatus> |]
            [| 0uy<CellStatus>; 1uy<CellStatus>; 0uy<CellStatus> |]
            [| 0uy<CellStatus>; 1uy<CellStatus>; 0uy<CellStatus> |]
        |]

        let ``expected array after the second iteration`` = [|
            [| 0uy<CellStatus>; 0uy<CellStatus>; 0uy<CellStatus> |]
            [| 1uy<CellStatus>; 1uy<CellStatus>; 1uy<CellStatus> |]
            [| 0uy<CellStatus>; 0uy<CellStatus>; 0uy<CellStatus> |]
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
