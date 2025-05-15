namespace Conway.Tests

open Conway.App.Raylib.File
open NUnit.Framework

module ``8-bit Bit Vector Tests`` =

    [<Test>]
    let ``Can create a zeroed bit vector`` () =
        let expectedByte = 0b00000000uy
        let actual = BitVector8.zeroed

        Assert.That(actual.Byte, Is.EqualTo expectedByte)

    [<Test>]
    let ``Can create an all-one bit vector`` () =
        let expectedByte = 0b11111111uy
        let actual = BitVector8.allOnes

        Assert.That(actual.Byte, Is.EqualTo expectedByte)

    [<Test>]
    let ``Can create a bit vector from a byte`` () =
        let expectedByte = 0b01010101uy
        let actual = BitVector8.createFromByte expectedByte

        Assert.That(actual.Byte, Is.EqualTo expectedByte)

    [<Test>]
    let ``Can read a true bit in a vector at a specified position`` () =
        let exampleByte = 0b01010101uy
        let bitVector = BitVector8.createFromByte exampleByte

        let bit = bitVector.ReadBitAt 0

        Assert.That(bit, Is.True)

    [<Test>]
    let ``Can read a false bit in a vector at a specified position`` () =
        let exampleByte = 0b01010101uy
        let bitVector = BitVector8.createFromByte exampleByte

        let bit = bitVector.ReadBitAt 1

        Assert.That(bit, Is.False)

    [<Test>]
    let ``Can set a bit in a vector at a specified position`` () =
        let exampleByte = 0b01010101uy
        let bitVector = BitVector8.createFromByte exampleByte

        let actual = bitVector.SetBitAt 1
        let expected = 0b01010111uy

        Assert.That(actual.Byte, Is.EqualTo expected)

    [<Test>]
    let ``Can clear a bit in a vector at a specified position`` () =
        let exampleByte = 0b01010101uy
        let bitVector = BitVector8.createFromByte exampleByte

        let actual = bitVector.ClearBitAt 0
        let expected = 0b01010100uy

        Assert.That(actual.Byte, Is.EqualTo expected)
