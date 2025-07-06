namespace Conway.App

open Raylib_cs

module Hotkeys =
    let mapKeyboardActions (ctx: ApplicationContext) = [|
        KeyboardKey.W, (fun _ -> ctx.Canvas.Camera.MoveCameraUp 1.0f)
        KeyboardKey.A, (fun _ -> ctx.Canvas.Camera.MoveCameraLeft 1.0f)
        KeyboardKey.S, (fun _ -> ctx.Canvas.Camera.MoveCameraDown 1.0f)
        KeyboardKey.D, (fun _ -> ctx.Canvas.Camera.MoveCameraRight 1.0f)
        KeyboardKey.Z, (fun _ -> ctx.Canvas.Camera.ZoomIn 0.2f)
        KeyboardKey.X, (fun _ -> ctx.Canvas.Camera.ZoomOut 0.2f)
    |]

    let mapKeyboardShiftActions (ctx: ApplicationContext) = [|
        KeyboardKey.W, (fun _ -> ctx.Canvas.Camera.MoveCameraUp 5.0f)
        KeyboardKey.A, (fun _ -> ctx.Canvas.Camera.MoveCameraLeft 5.0f)
        KeyboardKey.S, (fun _ -> ctx.Canvas.Camera.MoveCameraDown 5.0f)
        KeyboardKey.D, (fun _ -> ctx.Canvas.Camera.MoveCameraRight 5.0f)
    |]
