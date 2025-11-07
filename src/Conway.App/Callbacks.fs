namespace Conway.App

open Config
open Conway.App.Controls
open Conway.App.Utils.Alias
open Conway.Core
open Raylib_cs

module Callbacks =
    let saveFile (ctx: ApplicationContext) =
        try
            Run.saveFileProgram ctx
        with ex ->
            let excepionMessage = ex.Message.ToString().Replace("\n", "\n\t")
            Raylib.SetExitKey KeyboardKey.Escape

            Raylib.TraceLog(
                TraceLogLevel.Error,
                $"Failed to create a new savefile with the following error: {excepionMessage}"
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

    let randomize (ctx: ApplicationContext) =
        let currentGameMode = ctx.GameMode

        match currentGameMode with
        | GameState.Paused ->
            ctx.Canvas.Game.CurrentState <-
                ConwayGrid.createRandomWithOdds
                    ctx.Canvas.Game.CurrentState.ActiveWidth
                    ctx.Canvas.Game.CurrentState.ActiveHeight
                    Default.oddsOfGettingLivingCell
        | _ -> ()

    let toggleGame (ctx: ApplicationContext) =
        let currentGameMode = ctx.GameMode

        ctx.GameMode <-
            match currentGameMode with
            | GameState.Paused -> GameState.Infinite
            | _ -> GameState.Paused

    let advanceOnce (ctx: ApplicationContext) =
        match ctx.GameMode with
        | GameState.Paused -> ctx.Canvas.Game.RunOneStep()
        | _ -> ()

    let update (ctx: ApplicationContext) (button: Button) =
        match ctx.GameMode with
        | GameState.Paused -> button.Text <- "Run"
        | _ -> button.Text <- "Pause"

    let updateOnRun (ctx: ApplicationContext) (button: Button) =
        match ctx.GameMode with
        | GameState.Paused -> button.IsActive <- true
        | _ -> button.IsActive <- false

    let updateOnRunBack (ctx: ApplicationContext) (button: Button) =
        updateOnRun ctx button

        if button.IsActive && ctx.Canvas.Game.Generation <= 1 then
            button.IsActive <- false

    let resetCallback (ctx: ApplicationContext) =
        match ctx.GameMode with
        | GameState.Paused -> ctx.Canvas.Game.ResetState()
        | _ -> ()

    let clearCallback (ctx: ApplicationContext) =
        let grid = ctx.Canvas.Game.CurrentState

        match ctx.GameMode with
        | GameState.Paused -> ctx.Canvas.Game.CurrentState <- ConwayGrid.createDead grid.ActiveWidth grid.ActiveHeight
        | _ -> ()

    let fullscreenUpdate () =
        if raylibTrue (Raylib.IsKeyPressed KeyboardKey.Y) then
            if raylibTrue (Raylib.IsWindowFullscreen()) then
                Raylib.SetWindowSize(int Default.windowWidth, int Default.windowHeight)
            else
                let monitor = Raylib.GetCurrentMonitor()
                let monitorWidth = Raylib.GetMonitorWidth monitor
                let monitorHeight = Raylib.GetMonitorHeight monitor
                Raylib.SetWindowSize(monitorWidth, monitorHeight)

            Raylib.ToggleFullscreen()
