namespace Conway.Tests

open Conway.Encoding
open Conway.Core
open NUnit.Framework

module ``Binary Canvas File Saver Tests`` =

    [<Test>]
    let ``Can correctly encode grid dimensions`` () =
        let grid = ConwayGrid.createLiving 4 4

        let expectedEncoding = [|
            0b0000_0100uy // 4 rows (32-bit int)
            0b0000_0000uy // ...
            0b0000_0000uy // ...
            0b0000_0000uy // ...
            0b0000_0100uy // 4 cols (32-bit int)
            0b0000_0000uy // ...
            0b0000_0000uy // ...
            0b0000_0000uy // ...
        |]

        let encoder = new ConwayByteEncoder()
        let actualEncoding = encoder.EncodeDimensions grid

        CollectionAssert.AreEqual(expectedEncoding, actualEncoding)

    [<Test>]
    let ``Can correctly encode a living grid`` () =
        let grid = ConwayGrid.createLiving 4 4

        let expectedEncoding = [|
            0b1111_1111uy // 4x4 grid with all cells living
            0b1111_1111uy // ...
        |]

        let encoder = new ConwayByteEncoder()
        let actualEncoding = encoder.EncodeGrid grid

        CollectionAssert.AreEqual(expectedEncoding, actualEncoding)

    [<Test>]
    let ``Can correctly encode a dead grid`` () =
        let grid = ConwayGrid.createDead 4 4

        let expectedEncoding = [|
            0b0000_0000uy // 4x4 grid with all cells dead
            0b0000_0000uy // ...
        |]

        let encoder = new ConwayByteEncoder()
        let actualEncoding = encoder.EncodeGrid grid

        CollectionAssert.AreEqual(expectedEncoding, actualEncoding)

    [<Test>]
    let ``Can correctly encode a mixed grid`` () =
        let initializer i j =
            if (i * 4 + j) % 2 = 0 then 1<CellStatus> else 0<CellStatus>

        let grid = ConwayGrid.init 4 4 initializer

        let expectedEncoding = [|
            0b0101_0101uy // 4x4 grid with mixed dead/living cells
            0b0101_0101uy // ...
        |]

        let encoder = new ConwayByteEncoder()
        let actualEncoding = encoder.EncodeGrid grid

        CollectionAssert.AreEqual(expectedEncoding, actualEncoding)

    [<Test>]
    let ``Can correctly encode a game that hasn't started yet`` () =
        let game = Game(ConwayGrid.createDead 4 4)

        let expectedEncoding = [|
            0b0000_0100uy // 4 rows (32-bit int)
            0b0000_0000uy // ...
            0b0000_0000uy // ...
            0b0000_0000uy // ...
            0b0000_0100uy // 4 cols (32-bit int)
            0b0000_0000uy // ...
            0b0000_0000uy // ...
            0b0000_0000uy // ...
            0b0000_0000uy // 4x4 actual grid with all cells dead
            0b0000_0000uy // ...
            0b0000_0000uy // 4x4 initial grid with all cells dead
            0b0000_0000uy // ...
            0b0000_0001uy // generation counter (32-bit int)
            0b0000_0000uy // ...
            0b0000_0000uy // ...
            0b0000_0000uy // ...
        |]

        let encoder = new ConwayByteEncoder() :> IConwayByteEncoder
        let actualEncoding = encoder.Encode game

        CollectionAssert.AreEqual(expectedEncoding, actualEncoding)

    [<Test>]
    let ``Can correctly encode a game that has been run`` () =
        let initializer i j =
            if i = 1 && (j = 0 || j = 1 || j = 2) then
                1<CellStatus>
            else
                0<CellStatus>

        let game = Game(ConwayGrid.init 4 4 initializer)

        for _ in 1..3 do
            game.RunOneStep()

        let expectedEncoding = [|
            0b0000_0100uy // 4 rows (32-bit int)
            0b0000_0000uy // ...
            0b0000_0000uy // ...
            0b0000_0000uy // ...
            0b0000_0100uy // 4 cols (32-bit int)
            0b0000_0000uy // ...
            0b0000_0000uy // ...
            0b0000_0000uy // ...
            0b0010_0010uy // 4x4 actual grid
            0b0000_0010uy // ...
            0b0111_0000uy // 4x4 initial grid
            0b0000_0000uy // ...
            0b0000_0100uy // generation counter (32-bit int)
            0b0000_0000uy // ...
            0b0000_0000uy // ...
            0b0000_0000uy // ...
        |]

        let encoder = new ConwayByteEncoder() :> IConwayByteEncoder
        let actualEncoding = encoder.Encode game

        CollectionAssert.AreEqual(expectedEncoding, actualEncoding)
