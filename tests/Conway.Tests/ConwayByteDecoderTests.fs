namespace Conway.Tests

open Conway.Encoding
open Conway.Core
open NUnit.Framework

module ``Conway Byte Decoder Tests`` =

    [<Test>]
    let ``Can correctly decode grid dimensions`` () =
        let encoding = [|
            0b0000_0100uy // 4 rows (32-bit int)
            0b0000_0000uy // ...
            0b0000_0000uy // ...
            0b0000_0000uy // ...
            0b0000_0100uy // 4 cols (32-bit int)
            0b0000_0000uy // ...
            0b0000_0000uy // ...
            0b0000_0000uy // ...
        |]

        let decoder = ConwayByteDecoder()

        let expectedDimensions = 4, 4
        let actualDimensions = decoder.DecodeDimensions encoding

        Assert.That(actualDimensions, Is.EqualTo expectedDimensions)

    [<Test>]
    let ``Can correctly decode a living grid`` () =
        let encoding = [|
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

        let expectedGrid = ConwayGrid.createLiving 4 4

        let encoder = new ConwayByteDecoder()
        let actualGrid = encoder.DecodeGrid encoding

        CollectionAssert.AreEqual(expectedGrid.Board, actualGrid.Board)
