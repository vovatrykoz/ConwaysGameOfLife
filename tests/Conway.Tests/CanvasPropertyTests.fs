namespace Conway.Tests

open Conway.App
open Conway.App.Controls
open Conway.App.Math
open Conway.Core
open NUnit.Framework
open FsCheck.NUnit

[<Properties(Arbitrary = [| typeof<ConwayGen> |])>]
module ``Canvas Property Tests`` =
    //
    [<Property>]
    let ``A "Canvas" object is correctly initialized through its primary constructor``
        (x: float32<px>)
        (y: float32<px>)
        (width: float32<px>)
        (height: float32<px>)
        (camera: Camera)
        (game: Game)
        (cellSize: float32<px>)
        =
        let actual = Canvas(x, y, width, height, camera, game, cellSize)
        let expectedPosition = Vec2.create x y

        Assert.Multiple(fun _ ->
            Assert.That(actual.Position.X, Is.EqualTo expectedPosition.X)
            Assert.That(actual.Position.Y, Is.EqualTo expectedPosition.Y)
            Assert.That(actual.Width, Is.EqualTo width)
            Assert.That(actual.Height, Is.EqualTo height)
            Assert.That(actual.CellSize, Is.EqualTo cellSize)
            Assert.That(actual.Game, Is.EqualTo game)
            Assert.That(actual.Camera, Is.EqualTo camera))
