namespace Conway.Tests

open Conway.App
open Conway.App.Controls
open Conway.Core
open NUnit.Framework
open FsCheck
open FsCheck.NUnit
open System.Numerics

[<Properties(Arbitrary = [| typeof<ConwayGen> |])>]
module ``Canvas Property Tests`` =
    //
    [<Property>]
    let ``A "Canvas" object is correctly initialized through its primary constructor``
        (x: float32)
        (y: float32)
        (width: float32)
        (height: float32)
        (camera: Camera)
        (game: Game)
        (cellSize: float32)
        =
        let actual = Canvas(x, y, width, height, camera, game, cellSize)
        let expectedPosition = Vector2(x, y)

        Assert.Multiple(fun _ ->
            Assert.That(actual.Position, Is.EqualTo expectedPosition)
            Assert.That(actual.Width, Is.EqualTo width)
            Assert.That(actual.Height, Is.EqualTo height)
            Assert.That(actual.CellSize, Is.EqualTo cellSize)
            Assert.That(actual.Game, Is.EqualTo game)
            Assert.That(actual.Camera, Is.EqualTo camera))
