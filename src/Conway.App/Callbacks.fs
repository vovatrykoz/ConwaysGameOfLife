namespace Conway.App

open Config
open Conway.App.Controls
open Conway.App.File
open Conway.App.Utils.Alias
open Conway.Core
open Conway.Encoding
open Raylib_cs
open System
open System.IO

module Callbacks =
    open Conway.App.Graphics
    open Conway.App.Input
    open System.Collections.Generic

    let saveFile (ctx: ApplicationContext) =
        try
            Run.saveFileProgram ctx
        with ex ->
            let excepionMessage = ex.Message.ToString().Replace("\n", "\n\t")
            Raylib.SetExitKey KeyboardKey.Escape

            Raylib.TraceLog(
                TraceLogLevel.Error,
                $"Failed to create a directory with the following error: {excepionMessage}"
            )

    let openFile (ctx: ApplicationContext) =
        try
            Run.openFileProgram ctx
        with ex ->
            let excepionMessage = ex.Message.Replace("\n", "\n\t")
            let stackTrace = ex.StackTrace.Replace("\n", "\n\t")

            Raylib.TraceLog(
                TraceLogLevel.Error,
                $"Failed to load the save with the following error:\n\t{excepionMessage}\nStack trace:\n\t{stackTrace}"
            )

    let toggleGame (ctx: ApplicationContext) =
        let currentGameMode = ctx.GameMode

        ctx.GameMode <-
            match currentGameMode with
            | GameRunMode.Paused -> GameRunMode.Infinite
            | _ -> GameRunMode.Paused

    let advanceOnce (ctx: ApplicationContext) =
        match ctx.GameMode with
        | GameRunMode.Paused -> ctx.Canvas.Game.RunOneStep()
        | _ -> ()

    let update (ctx: ApplicationContext) (button: Button) =
        match ctx.GameMode with
        | GameRunMode.Paused -> button.Text <- "Run"
        | _ -> button.Text <- "Pause"

    let updateOnRun (ctx: ApplicationContext) (button: Button) =
        match ctx.GameMode with
        | GameRunMode.Paused -> button.IsActive <- true
        | _ -> button.IsActive <- false

    let updateOnRunBack (ctx: ApplicationContext) (button: Button) =
        updateOnRun ctx button

        if button.IsActive && ctx.Canvas.Game.Generation <= 1 then
            button.IsActive <- false

    let resetCallback (ctx: ApplicationContext) =
        match ctx.GameMode with
        | GameRunMode.Paused -> ctx.Canvas.Game.ResetState()
        | _ -> ()

    let clearCallback (ctx: ApplicationContext) =
        let grid = ctx.Canvas.Game.CurrentState

        match ctx.GameMode with
        | GameRunMode.Paused -> ctx.Canvas.Game.CurrentState <- ConwayGrid.createDead grid.ActiveWidth grid.ActiveHeight
        | _ -> ()

    let fullscreenUpdate () =
        if raylibTrue (Raylib.IsKeyPressed KeyboardKey.Y) then
            if raylibTrue (Raylib.IsWindowFullscreen()) then
                Raylib.SetWindowSize(Default.windowWidth, Default.windowHeight)
            else
                let monitor = Raylib.GetCurrentMonitor()
                let monitorWidth = Raylib.GetMonitorWidth monitor
                let monitorHeight = Raylib.GetMonitorHeight monitor
                Raylib.SetWindowSize(monitorWidth, monitorHeight)

            Raylib.ToggleFullscreen()
