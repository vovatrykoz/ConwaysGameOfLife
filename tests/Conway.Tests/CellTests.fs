module Conway.Tests

open Conway.Core
open NUnit.Framework

module ``Cell Tests`` =

    [<Test>]
    let ``Can create a living cell using the dedicated method on the Cell type`` () =
        let expected = { Status = Alive }
        let actual = Cell.createLivingCell

        Assert.That(actual, Is.EqualTo expected)

    [<Test>]
    let ``Can create a dead cell using the dedicated method on the Cell type`` () =
        let expected = { Status = Dead }
        let actual = Cell.createDeadCell

        Assert.That(actual, Is.EqualTo expected)

module ``Grid tests`` =

    [<Test>]
    let ``Can create a simple grid using the dedicated method on the Grid type`` () =
        let expectedArray = [|
            [| BorderCell; BorderCell; BorderCell; BorderCell |]
            [|
                BorderCell
                PlayerCell Cell.createDeadCell
                PlayerCell Cell.createDeadCell
                BorderCell
            |]
            [|
                BorderCell
                PlayerCell Cell.createDeadCell
                PlayerCell Cell.createDeadCell
                BorderCell
            |]
            [| BorderCell; BorderCell; BorderCell; BorderCell |]
        |]

        let expectedBoard = Array2D.init 4 4 (fun i j -> expectedArray[i][j])

        let actual = Grid.createDead 2 2

        Assert.That(actual.Board, Is.EqualTo expectedBoard)

    [<Test>]
    let ``Can create a living grid using the dedicated method on the Grid type`` () =
        let expectedArray = [|
            [| BorderCell; BorderCell; BorderCell; BorderCell |]
            [|
                BorderCell
                PlayerCell Cell.createLivingCell
                PlayerCell Cell.createLivingCell
                BorderCell
            |]
            [|
                BorderCell
                PlayerCell Cell.createLivingCell
                PlayerCell Cell.createLivingCell
                BorderCell
            |]
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
                PlayerCell Cell.createLivingCell
                PlayerCell Cell.createDeadCell
                PlayerCell Cell.createLivingCell
                BorderCell
            |]
            [|
                BorderCell
                PlayerCell Cell.createDeadCell
                PlayerCell Cell.createLivingCell
                PlayerCell Cell.createDeadCell
                BorderCell
            |]
            [|
                BorderCell
                PlayerCell Cell.createLivingCell
                PlayerCell Cell.createDeadCell
                PlayerCell Cell.createLivingCell
                BorderCell
            |]
            [| BorderCell; BorderCell; BorderCell; BorderCell; BorderCell |]
        |]

        let initializer i j =
            if (i + j) % 2 = 0 then
                Cell.createLivingCell
            else
                Cell.createDeadCell

        let expectedBoard = Array2D.init 5 5 (fun i j -> expectedArray[i][j])

        let actual = Grid.init 3 3 initializer

        Assert.That(actual.Board, Is.EqualTo expectedBoard)
