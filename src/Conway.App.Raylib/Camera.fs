namespace Conway.App.Raylib

open System.Numerics

type Camera(x: float32, y: float32) =
    let mutable _position = Vector2(x, y)

    member _.Position
        with get () = _position
        and set newPosition = _position <- newPosition

    member val ZoomFactor = 1.0f with get, set

    member val MaxZoomFactor = 2.0f with get, set

    member val MinZoomFactor = 0.2f with get, set

    member _.MoveCameraRight(speed: float32) = _position.X <- _position.X + speed

    member _.MoveCameraLeft(speed: float32) = _position.X <- _position.X - speed

    member _.MoveCameraUp(speed: float32) = _position.Y <- _position.Y - speed

    member _.MoveCameraDown(speed: float32) = _position.Y <- _position.Y + speed

    member this.ZoomIn(speed: float32) =
        this.ZoomFactor <- min (this.ZoomFactor + speed) this.MaxZoomFactor

    member this.ZoomOut(speed: float32) =
        this.ZoomFactor <- max (this.ZoomFactor - speed) this.MinZoomFactor
