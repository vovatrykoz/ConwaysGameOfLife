module Conway.Tests

open Conway.Core
open NUnit.Framework

module ``Cell Tests`` =

    [<Test>]
    let ``Can create a living cell using the dedicated method on the Cell type`` () =
        let expected = { Status = Alive }
        let actual = Cell.living

        Assert.That(actual, Is.EqualTo expected)

    [<Test>]
    let ``Can create a dead cell using the dedicated method on the Cell type`` () =
        let expected = { Status = Dead }
        let actual = Cell.dead

        Assert.That(actual, Is.EqualTo expected)

module ``Grid tests`` =

    [<Test>]
    let ``Can create a simple grid using the dedicated method on the Grid type`` () =
        let expectedArray = [|
            [| BorderCell; BorderCell; BorderCell; BorderCell |]
            [| BorderCell; PlayerCell Cell.dead; PlayerCell Cell.dead; BorderCell |]
            [| BorderCell; PlayerCell Cell.dead; PlayerCell Cell.dead; BorderCell |]
            [| BorderCell; BorderCell; BorderCell; BorderCell |]
        |]

        let expectedBoard = Array2D.init 4 4 (fun i j -> expectedArray[i][j])

        let actual = Grid.createDead 2 2

        Assert.That(actual.Board, Is.EqualTo expectedBoard)

    [<Test>]
    let ``Can create a living grid using the dedicated method on the Grid type`` () =
        let expectedArray = [|
            [| BorderCell; BorderCell; BorderCell; BorderCell |]
            [| BorderCell; PlayerCell Cell.living; PlayerCell Cell.living; BorderCell |]
            [| BorderCell; PlayerCell Cell.living; PlayerCell Cell.living; BorderCell |]
            [| BorderCell; BorderCell; BorderCell; BorderCell |]
        |]

        let expectedBoard = Array2D.init 4 4 (fun i j -> expectedArray[i][j])

        let actual = Grid.createLiving 2 2

        Assert.That(actual.Board, Is.EqualTo expectedBoard)

    [<Test>]
    let ``Can create a living grid using the init method on the Grid type`` () =
        let expectedArray = [|
            [| BorderCell; BorderCell; BorderCell; BorderCell; BorderCell |]
            [|
                BorderCell
                PlayerCell Cell.living
                PlayerCell Cell.dead
                PlayerCell Cell.living
                BorderCell
            |]
            [|
                BorderCell
                PlayerCell Cell.dead
                PlayerCell Cell.living
                PlayerCell Cell.dead
                BorderCell
            |]
            [|
                BorderCell
                PlayerCell Cell.living
                PlayerCell Cell.dead
                PlayerCell Cell.living
                BorderCell
            |]
            [| BorderCell; BorderCell; BorderCell; BorderCell; BorderCell |]
        |]

        let initializer i j =
            if (i + j) % 2 = 0 then Cell.living else Cell.dead

        let expectedBoard = Array2D.init 5 5 (fun i j -> expectedArray[i][j])

        let actual = Grid.init 3 3 initializer

        Assert.That(actual.Board, Is.EqualTo expectedBoard)

    [<Test>]
    let ``All-dead grid remains dead after one iteration`` () =
        let expectedBoard = Grid.createDead 3 3

        let actual = Grid.createDead 3 3 |> Grid.next

        Assert.That(actual, Is.EqualTo expectedBoard)

    [<Test>]
    let ``A cell dies with no living neighbors`` () =
        let initializer i j =
            if i = 1 && j = 1 then Cell.living else Cell.dead

        let expectedBoard = Grid.createDead 3 3

        let actual = Grid.init 3 3 initializer |> Grid.next

        Assert.That(actual, Is.EqualTo expectedBoard)

    [<Test>]
    let ``A cell dies with one living neighbors`` () =
        let initializer i j =
            if i = 1 && j = 1 || i = 2 && j = 2 then
                Cell.living
            else
                Cell.dead

        let expectedBoard = Grid.createDead 3 3

        let actual = Grid.init 3 3 initializer |> Grid.next

        Assert.That(actual, Is.EqualTo expectedBoard)

    [<Test>]
    let ``A cell becomes alive with three living neighbors`` () =
        let setup = [| [| Cell.living; Cell.living |]; [| Cell.living; Cell.dead |] |]
        let expectedArray = [| [| Cell.living; Cell.living |]; [| Cell.living; Cell.living |] |]

        let setupInitializer i j = setup[i][j]
        let expectedInitializer i j = expectedArray[i][j]

        let actual = Grid.init 2 2 setupInitializer |> Grid.next
        let expectedBoard = Grid.init 2 2 expectedInitializer

        Assert.That(actual, Is.EqualTo expectedBoard)

    [<Test>]
    let ``A living cell with three living neighbors keeps on living`` () =
        let setup = [| [| Cell.living; Cell.living |]; [| Cell.living; Cell.living |] |]
        let expectedArray = [| [| Cell.living; Cell.living |]; [| Cell.living; Cell.living |] |]

        let setupInitializer i j = setup[i][j]
        let expectedInitializer i j = expectedArray[i][j]

        let actual = Grid.init 2 2 setupInitializer |> Grid.next
        let expectedBoard = Grid.init 2 2 expectedInitializer

        Assert.That(actual, Is.EqualTo expectedBoard)

    [<Test>]
    let ``A cell with four living neighbors dies`` () =
        let setup = [|
            [| Cell.living; Cell.living; Cell.living |]
            [| Cell.living; Cell.living; Cell.dead |]
        |]

        let expectedArray = [|
            [| Cell.living; Cell.dead; Cell.living |]
            [| Cell.living; Cell.dead; Cell.living |]
        |]

        let setupInitializer i j = setup[i][j]
        let expectedInitializer i j = expectedArray[i][j]

        let actual = Grid.init 3 2 setupInitializer |> Grid.next
        let expectedBoard = Grid.init 3 2 expectedInitializer

        Assert.That(actual, Is.EqualTo expectedBoard)

    [<Test>]
    let ``Three cells in a row stay alive by switching to rows and columns`` () =
        let setup = [|
            [| Cell.dead; Cell.dead; Cell.dead |]
            [| Cell.living; Cell.living; Cell.living |]
            [| Cell.dead; Cell.dead; Cell.dead |]
        |]

        let ``expected array after the first iteration`` = [|
            [| Cell.dead; Cell.living; Cell.dead |]
            [| Cell.dead; Cell.living; Cell.dead |]
            [| Cell.dead; Cell.living; Cell.dead |]
        |]

        let ``expected array after the second iteration`` = [|
            [| Cell.dead; Cell.dead; Cell.dead |]
            [| Cell.living; Cell.living; Cell.living |]
            [| Cell.dead; Cell.dead; Cell.dead |]
        |]

        let setupInitializer i j = setup[i][j]

        let expectedInitializerOne i j =
            ``expected array after the first iteration``[i][j]

        let expectedInitializerTwo i j =
            ``expected array after the second iteration``[i][j]

        let actual = Grid.init 3 3 setupInitializer |> Grid.next
        let expectedBoardOne = Grid.init 3 3 expectedInitializerOne

        Assert.That(actual, Is.EqualTo expectedBoardOne)

        let actual = actual |> Grid.next
        let expectedBoardTwo = Grid.init 3 3 expectedInitializerTwo

        Assert.That(actual, Is.EqualTo expectedBoardTwo)
