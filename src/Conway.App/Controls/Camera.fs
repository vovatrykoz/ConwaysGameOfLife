namespace Conway.App.Controls

open Conway.App.Math

/// <summary>
/// Represents a 2D camera with position and zoom control for navigating the game grid.
/// </summary>
/// <param name="x">Initial x-coordinate of the camera in cells.</param>
/// <param name="y">Initial y-coordinate of the camera in cells.</param>
/// <param name="zoomFactor">Initial zoom factor of the camera.</param>
type Camera(x: float32<cells>, y: float32<cells>, zoomFactor: float32) =
    let mutable _position = Vec2.create x y

    /// <summary>Secondary constructor with default zoom factor of 1.0.</summary>
    new(x: float32<cells>, y: float32<cells>) = Camera(x, y, 1.0f)

    /// <summary>Current position of the camera in cells.</summary>
    member _.Position
        with get () = _position
        and set newPosition = _position <- newPosition

    /// <summary>Current zoom factor of the camera.</summary>
    member val ZoomFactor = zoomFactor with get, set

    /// <summary>Maximum allowed zoom factor.</summary>
    member val MaxZoomFactor = 2.0f with get

    /// <summary>Minimum allowed zoom factor.</summary>
    member val MinZoomFactor = 0.2f with get

    /// <summary>Moves the camera to the right by the specified speed in cells.</summary>
    member _.MoveCameraRight(speed: float32<cells>) = _position.X <- _position.X + speed

    /// <summary>Moves the camera to the left by the specified speed in cells.</summary>
    member _.MoveCameraLeft(speed: float32<cells>) = _position.X <- _position.X - speed

    /// <summary>Moves the camera up by the specified speed in cells.</summary>
    member _.MoveCameraUp(speed: float32<cells>) = _position.Y <- _position.Y - speed

    /// <summary>Moves the camera down by the specified speed in cells.</summary>
    member _.MoveCameraDown(speed: float32<cells>) = _position.Y <- _position.Y + speed

    /// <summary>Zooms the camera in by the specified speed, clamped to the maximum zoom factor.</summary>
    /// <param name="speed">Zoom increment value.</param>
    member this.ZoomIn(speed: float32) =
        this.ZoomFactor <- min (this.ZoomFactor + speed) this.MaxZoomFactor

    /// <summary>Zooms the camera out by the specified speed, clamped to the minimum zoom factor.</summary>
    /// <param name="speed">Zoom decrement value.</param>
    member this.ZoomOut(speed: float32) =
        this.ZoomFactor <- max (this.ZoomFactor - speed) this.MinZoomFactor
