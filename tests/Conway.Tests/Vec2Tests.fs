namespace Conway.Tests

open Conway.App.Math
open NUnit.Framework
open FsCheck
open FsCheck.NUnit
open System.Runtime.CompilerServices
open System.Numerics

// Warning FS0042: This construct is deprecated: it is only for use in the F# library
#nowarn "42"

[<RequireQualifiedAccess>]
module private UnsafeUtils =

    let inline retype<'T, 'U> (x: 'T) : 'U = (# "" x: 'U #)

    let inline pointersAreEqual (ptr1: voidptr) (ptr2: voidptr) : bool = (# "ceq" ptr1 ptr2 : bool #)

[<Properties(Arbitrary = [| typeof<ConwayGen> |])>]
module ``Vec2 Tests`` =
    //
    [<Property>]
    let ``Vec2 is initialized correctly`` (x: float32<px>) (y: float32<px>) =
        let vec = Vec2.create x y

        Assert.Multiple(fun _ ->
            Assert.That(vec.X, Is.EqualTo x)
            Assert.That(vec.Y, Is.EqualTo y))

    [<Property>]
    let ``Two Vec2s are added coordwise using the static add member`` (v1: Vec2<px>) (v2: Vec2<px>) =
        let sum = Vec2.add v1 v2

        let expectedX = v1.X + v2.X
        let expectedY = v1.Y + v2.Y

        Assert.Multiple(fun _ ->
            Assert.That(sum.X, Is.EqualTo expectedX)
            Assert.That(sum.Y, Is.EqualTo expectedY))

    [<Property>]
    let ``Two Vec2s are added coordwise using the addition operator`` (v1: Vec2<px>) (v2: Vec2<px>) =
        let sum = v1 + v2

        let expectedX = v1.X + v2.X
        let expectedY = v1.Y + v2.Y

        Assert.Multiple(fun _ ->
            Assert.That(sum.X, Is.EqualTo expectedX)
            Assert.That(sum.Y, Is.EqualTo expectedY))

    [<Property>]
    let ``Two Vec2s are subtracted coordwise using the static subtract member`` (v1: Vec2<px>) (v2: Vec2<px>) =
        let sum = Vec2.subtract v1 v2

        let expectedX = v1.X - v2.X
        let expectedY = v1.Y - v2.Y

        Assert.Multiple(fun _ ->
            Assert.That(sum.X, Is.EqualTo expectedX)
            Assert.That(sum.Y, Is.EqualTo expectedY))

    [<Property>]
    let ``Two Vec2s are subtracted coordwise using the subtraction operator`` (v1: Vec2<px>) (v2: Vec2<px>) =
        let sum = v1 - v2

        let expectedX = v1.X - v2.X
        let expectedY = v1.Y - v2.Y

        Assert.Multiple(fun _ ->
            Assert.That(sum.X, Is.EqualTo expectedX)
            Assert.That(sum.Y, Is.EqualTo expectedY))

    [<Property>]
    let ``Two Vec2s are multiplied coordwise using the static multCoordwise member`` (v1: Vec2<px>) (v2: Vec2<1>) =
        let result = Vec2.multCoordwise v1 v2

        let expectedX = v1.X * v2.X
        let expectedY = v1.Y * v2.Y

        Assert.Multiple(fun _ ->
            Assert.That(result.X, Is.EqualTo expectedX)
            Assert.That(result.Y, Is.EqualTo expectedY))

    [<Property>]
    let ``Two Vec2s are multiplied coordwise using the multiplication operator`` (v1: Vec2<px>) (v2: Vec2<1>) =
        let result = v1 * v2

        let expectedX = v1.X * v2.X
        let expectedY = v1.Y * v2.Y

        Assert.Multiple(fun _ ->
            Assert.That(result.X, Is.EqualTo expectedX)
            Assert.That(result.Y, Is.EqualTo expectedY))

    [<Property>]
    let ``Vec2 can be scaled by a scalar value`` (v: Vec2<px>) (value: float32) =
        let result = v |> Vec2.scaleBy value

        let expectedX = v.X * value
        let expectedY = v.Y * value

        Assert.Multiple(fun _ ->
            Assert.That(result.X, Is.EqualTo expectedX)
            Assert.That(result.Y, Is.EqualTo expectedY))

    [<Property>]
    let ``Vec2 can be crated by transforming a System Numerics Vector2`` (original: Vector2) =
        let transformed = Vec2<px>.fromNumericVector original

        Assert.Multiple(fun _ ->
            Assert.That(transformed.X, Is.EqualTo original.X)
            Assert.That(transformed.Y, Is.EqualTo original.Y))
