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
