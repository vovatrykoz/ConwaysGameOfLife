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
