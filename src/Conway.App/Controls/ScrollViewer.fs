namespace Conway.App.Controls

open Conway.App.Math

type ScrollViewer(x: float32<px>, y: float32<px>, zoomFactor: float32) =
    let mutable _position = Vec2.create x y

    new(x: float32<px>, y: float32<px>) = ScrollViewer(x, y, 1.0f)

    member _.Position
        with get () = _position
        and set newPosition = _position <- newPosition

    member val ZoomFactor = zoomFactor with get, set

    member val MaxZoomFactor = 2.0f with get

    member val MinZoomFactor = 0.2f with get

    member _.ScrollRight(speed: float32<px>) = _position.X <- _position.X + speed

    member _.ScrollLeft(speed: float32<px>) = _position.X <- _position.X - speed

    member _.ScrollUp(speed: float32<px>) = _position.Y <- _position.Y - speed

    member _.ScrollDown(speed: float32<px>) = _position.Y <- _position.Y + speed

    member this.ZoomIn(speed: float32) =
        this.ZoomFactor <- min (this.ZoomFactor + speed) this.MaxZoomFactor

    member this.ZoomOut(speed: float32) =
        this.ZoomFactor <- max (this.ZoomFactor - speed) this.MinZoomFactor
