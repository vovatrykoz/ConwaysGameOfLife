module Conway.Tests

open Conway.Core
open NUnit.Framework
(*
module ``Cell Tests`` =

    [<Test>]
    let ``Can create a living cell using the dedicated method on the Cell type`` () =
        let expected = { Status = 0<cellStatus> }
        let actual = Cell.living

        Assert.That(actual, Is.EqualTo expected)

    [<Test>]
    let ``Can create a 0 cell using the dedicated method on the Cell type`` () =
        let expected = { Status = 0<cellStatus> }
        let actual = Cell.dead

        Assert.That(actual, Is.EqualTo expected)

module ``Grid tests`` =

    [<Test>]
    let ``Can create a simple grid using the dedicated method on the Grid type`` () =
        let expectedArray = [|
            [| Cell.dead; Cell.dead; Cell.dead; Cell.dead |]
            [| Cell.dead; Cell.dead; Cell.dead; Cell.dead |]
            [| Cell.dead; Cell.dead; Cell.dead; Cell.dead |]
            [| Cell.dead; Cell.dead; Cell.dead; Cell.dead |]
        |]

        let expectedBoard = Array2D.init 4 4 (fun i j -> expectedArray[i][j])

        let actual = ConwayGrid.createDead 2 2

        Assert.That(actual.Board, Is.EqualTo expectedBoard)

    [<Test>]
    let ``Can create a living grid using the dedicated method on the Grid type`` () =
        let expectedArray = [|
            [| Cell.dead; Cell.dead; Cell.dead; Cell.dead |]
            [| Cell.dead; Cell.living; Cell.living; Cell.dead |]
            [| Cell.dead; Cell.living; Cell.living; Cell.dead |]
            [| Cell.dead; Cell.dead; Cell.dead; Cell.dead |]
        |]

        let expectedBoard = Array2D.init 4 4 (fun i j -> expectedArray[i][j])

        let actual = ConwayGrid.createLiving 2 2

        Assert.That(actual.Board, Is.EqualTo expectedBoard)

    [<Test>]
    let ``Can create a living grid using the init method on the Grid type`` () =
        let expectedArray = [|
            [| Cell.dead; Cell.dead; Cell.dead; Cell.dead; Cell.dead |]
            [| Cell.dead; Cell.living; Cell.dead; Cell.living; Cell.dead |]
            [| Cell.dead; Cell.dead; Cell.living; Cell.dead; Cell.dead |]
            [| Cell.dead; Cell.living; Cell.dead; Cell.living; Cell.dead |]
            [| Cell.dead; Cell.dead; Cell.dead; Cell.dead; Cell.dead |]
        |]

        let initializer i j =
            if (i + j) % 2 = 0 then Cell.living else Cell.dead

        let expectedBoard = Array2D.init 5 5 (fun i j -> expectedArray[i][j])

        let actual = ConwayGrid.init 3 3 initializer

        Assert.That(actual.Board, Is.EqualTo expectedBoard)

    [<Test>]
    let ``All-dead grid remains 0 after one iteration`` () =
        let initializer _ _ = Cell.create 0<cellStatus>

        let expectedBoard = (ConwayGrid.init 3 3 initializer).Board

        let actual = ConwayGrid.createDead 3 3
        actual.AdvanceToNextState()

        Assert.That(actual.Board, Is.EqualTo expectedBoard)

    [<Test>]
    let ``A cell dies with no living neighbors`` () =
        let initializerForExpected i j =
            if i = 1 && j = 1 then
                Cell.create 0<cellStatus>
            else
                Cell.create 0<cellStatus>

        let initializerForActual i j =
            if i = 1 && j = 1 then Cell.living else Cell.dead

        let expectedBoard = (ConwayGrid.init 3 3 initializerForExpected).Board

        let actual = ConwayGrid.init 3 3 initializerForActual
        actual.AdvanceToNextState()

        Assert.That(actual.Board, Is.EqualTo expectedBoard)

    [<Test>]
    let ``A cell dies with one living neighbors`` () =
        let initializerForExpected i j =
            if i = 1 && j = 1 || i = 2 && j = 2 then
                Cell.create 0<cellStatus>
            else
                Cell.create 0<cellStatus>

        let initializerForActual i j =
            if i = 1 && j = 1 || i = 2 && j = 2 then
                Cell.living
            else
                Cell.dead

        let expectedBoard = (ConwayGrid.init 3 3 initializerForExpected).Board

        let actual = ConwayGrid.init 3 3 initializerForActual
        actual.AdvanceToNextState()

        Assert.That(actual.Board, Is.EqualTo expectedBoard)

    [<Test>]
    let ``A cell becomes 0 with three living neighbors`` () =
        let setup = [| [| Cell.living; Cell.living |]; [| Cell.living; Cell.dead |] |]

        let expectedArray = [|
            [| Cell.create 0<cellStatus>; Cell.create 0<cellStatus> |]
            [| Cell.create 0<cellStatus>; Cell.create 0<cellStatus> |]
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
        let setup = [| [| Cell.living; Cell.living |]; [| Cell.living; Cell.living |] |]

        let expectedArray = [|
            [| Cell.create 0<cellStatus>; Cell.create 0<cellStatus> |]
            [| Cell.create 0<cellStatus>; Cell.create 0<cellStatus> |]
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
            [| Cell.living; Cell.living; Cell.living |]
            [| Cell.living; Cell.living; Cell.dead |]
        |]

        // the two middle cells both have 4 living neighbors, therefore both are expected to die
        // The cell in the bottom right should become 0, as it had three living neighbors
        let expectedArray = [|
            [|
                Cell.create 0<cellStatus>
                Cell.create 0<cellStatus>
                Cell.create 0<cellStatus>
            |]
            [|
                Cell.create 0<cellStatus>
                Cell.create 0<cellStatus>
                Cell.create 0<cellStatus>
            |]
        |]

        let setupInitializer i j = setup[i][j]
        let expectedInitializer i j = expectedArray[i][j]

        let actual = ConwayGrid.init 3 2 setupInitializer
        actual.AdvanceToNextState()

        let expectedBoard = (ConwayGrid.init 3 2 expectedInitializer).Board

        Assert.That(actual.Board, Is.EqualTo expectedBoard)

    [<Test>]
    let ``Three cells in a row stay 0 by switching to rows and columns`` () =
        let setup = [|
            [| Cell.dead; Cell.dead; Cell.dead |]
            [| Cell.living; Cell.living; Cell.living |]
            [| Cell.dead; Cell.dead; Cell.dead |]
        |]

        let ``expected array after the first iteration`` = [|
            [|
                Cell.create 0<cellStatus>
                Cell.create 0<cellStatus>
                Cell.create 0<cellStatus>
            |]
            [|
                Cell.create 0<cellStatus>
                Cell.create 0<cellStatus>
                Cell.create 0<cellStatus>
            |]
            [|
                Cell.create 0<cellStatus>
                Cell.create 0<cellStatus>
                Cell.create 0<cellStatus>
            |]
        |]

        let ``expected array after the second iteration`` = [|
            [|
                Cell.create 0<cellStatus>
                Cell.create 0<cellStatus>
                Cell.create 0<cellStatus>
            |]
            [|
                Cell.create 0<cellStatus>
                Cell.create 0<cellStatus>
                Cell.create 0<cellStatus>
            |]
            [|
                Cell.create 0<cellStatus>
                Cell.create 0<cellStatus>
                Cell.create 0<cellStatus>
            |]
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
*)
