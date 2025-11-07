namespace Conway.Tests

open Conway.App
open Conway.App.Controls
open Conway.App.Math
open Conway.Core
open NUnit.Framework
open FsCheck.NUnit

[<Properties(Arbitrary = [| typeof<ConwayGen> |])>]
module ``Camera Property Tests`` =
    //
    [<Property>]
    let ``Camera constructor sets all the values correctly``
        (x: float32<cells>, y: float32<cells>, zoomFactor: float32)
        =
        let camera = new Camera(x, y, zoomFactor)

        Assert.Multiple(fun _ ->
            Assert.That(camera.Position.X, Is.EqualTo x)
            Assert.That(camera.Position.Y, Is.EqualTo y)
            Assert.That(camera.ZoomFactor, Is.EqualTo zoomFactor))

    [<Property>]
    let ``Camera can be correctly moved to the right`` (camera: Camera) (speed: float32<cells>) =
        let expected =
            Camera(camera.Position.X + speed, camera.Position.Y, camera.ZoomFactor)

        camera.MoveCameraRight speed

        Assert.Multiple(fun _ ->
            Assert.That(camera.Position.X, Is.EqualTo expected.Position.X)
            Assert.That(camera.Position.Y, Is.EqualTo expected.Position.Y)
            Assert.That(camera.ZoomFactor, Is.EqualTo expected.ZoomFactor))

    [<Property>]
    let ``Camera can be correctly moved to the left`` (camera: Camera) (speed: float32<cells>) =
        let expected =
            Camera(camera.Position.X - speed, camera.Position.Y, camera.ZoomFactor)

        camera.MoveCameraLeft speed

        Assert.Multiple(fun _ ->
            Assert.That(camera.Position.X, Is.EqualTo expected.Position.X)
            Assert.That(camera.Position.Y, Is.EqualTo expected.Position.Y)
            Assert.That(camera.ZoomFactor, Is.EqualTo expected.ZoomFactor))

    [<Property>]
    let ``Camera can be correctly moved up`` (camera: Camera) (speed: float32<cells>) =
        let expected =
            Camera(camera.Position.X, camera.Position.Y - speed, camera.ZoomFactor)

        camera.MoveCameraUp speed

        Assert.Multiple(fun _ ->
            Assert.That(camera.Position.X, Is.EqualTo expected.Position.X)
            Assert.That(camera.Position.Y, Is.EqualTo expected.Position.Y)
            Assert.That(camera.ZoomFactor, Is.EqualTo expected.ZoomFactor))

    [<Property>]
    let ``Camera can be correctly moved down`` (camera: Camera) (speed: float32<cells>) =
        let expected =
            Camera(camera.Position.X, camera.Position.Y + speed, camera.ZoomFactor)

        camera.MoveCameraDown speed

        Assert.Multiple(fun _ ->
            Assert.That(camera.Position.X, Is.EqualTo expected.Position.X)
            Assert.That(camera.Position.Y, Is.EqualTo expected.Position.Y)
            Assert.That(camera.ZoomFactor, Is.EqualTo expected.ZoomFactor))

    [<Property>]
    let ``Camera can be correctly zoomed in`` (camera: Camera) (zoomFactor: float32) =
        let expected =
            Camera(camera.Position.X, camera.Position.Y, min (camera.ZoomFactor + zoomFactor) camera.MaxZoomFactor)

        camera.ZoomIn zoomFactor

        Assert.Multiple(fun _ ->
            Assert.That(camera.Position.X, Is.EqualTo expected.Position.X)
            Assert.That(camera.Position.Y, Is.EqualTo expected.Position.Y)
            Assert.That(camera.ZoomFactor, Is.EqualTo expected.ZoomFactor))

    [<Property>]
    let ``Camera can be correctly zoomed out`` (camera: Camera) (zoomFactor: float32) =
        let expected =
            Camera(camera.Position.X, camera.Position.Y, max (camera.ZoomFactor - zoomFactor) camera.MinZoomFactor)

        camera.ZoomOut zoomFactor

        Assert.Multiple(fun _ ->
            Assert.That(camera.Position.X, Is.EqualTo expected.Position.X)
            Assert.That(camera.Position.Y, Is.EqualTo expected.Position.Y)
            Assert.That(camera.ZoomFactor, Is.EqualTo expected.ZoomFactor))
