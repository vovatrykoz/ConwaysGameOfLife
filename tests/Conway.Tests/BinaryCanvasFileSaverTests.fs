namespace Conway.Tests

open Conway.App.Raylib.File
open Conway.Core
open NUnit.Framework

module ``Binary Canvas File Saver Tests`` =

    [<Test>]
    let ``Can correctly encode a living grid`` () =
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
            0b1111_1111uy // 4x4 grid with all cells living
            0b1111_1111uy // ...
        |]

        let fileSaver = new BinaryCanvasFileSaver()
        let actualEncoding = fileSaver.Encode grid

        CollectionAssert.AreEqual(expectedEncoding, actualEncoding)

    [<Test>]
    let ``Can correctly encode a dead grid`` () =
        let grid = ConwayGrid.createDead 4 4

        let expectedEncoding = [|
            0b0000_0100uy // 4 rows (32-bit int)
            0b0000_0000uy // ...
            0b0000_0000uy // ...
            0b0000_0000uy // ...
            0b0000_0100uy // 4 cols (32-bit int)
            0b0000_0000uy // ...
            0b0000_0000uy // ...
            0b0000_0000uy // ...
            0b0000_0000uy // 4x4 grid with all cells dead
            0b0000_0000uy // ...
        |]

        let fileSaver = new BinaryCanvasFileSaver()
        let actualEncoding = fileSaver.Encode grid

        CollectionAssert.AreEqual(expectedEncoding, actualEncoding)
