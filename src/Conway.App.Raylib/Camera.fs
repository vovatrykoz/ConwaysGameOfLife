namespace Conway.App.Raylib

type Camera(x: float32, y: float32) =
    member val X = x with get, set

    member val Y = y with get, set

    member this.MoveCameraRight(speed: float32) = this.X <- this.X - speed

    member this.MoveCameraLeft(speed: float32) = this.X <- this.X + speed

    member this.MoveCameraUp(speed: float32) = this.Y <- this.Y + speed

    member this.MoveCameraDown(speed: float32) = this.Y <- this.Y - speed
