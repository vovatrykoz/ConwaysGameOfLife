open Conway.App
open Conway.App.Config
open Conway.App.Controls
open Conway.App.Graphics
open Conway.App.Utils.Alias
open Conway.Core
open Raylib_cs
open System
open System.Diagnostics

Display.init Default.windowWidth Default.windowHeight
Display.loadingScreen (float32 (Default.windowWidth / 2)) (float32 (Default.windowHeight / 2))

let args = Environment.GetCommandLineArgs()

let userInput = UserInput.tryReadArgs args

match userInput with
| Error err ->
    match err with
    | UnknownSwitch sw -> Raylib.TraceLog(TraceLogLevel.Error, $"Unknown switch {sw}. Terminating the program")
    | NoWidthProvided ->
        Raylib.TraceLog(
            TraceLogLevel.Error,
            $"No width value provided after the width switch (-w|--width) was used. Terminating the program"
        )
    | NoHeightProvided ->
        Raylib.TraceLog(
            TraceLogLevel.Error,
            $"No height value provided after the height switch (-h|--height) was used. Terminating the program"
        )

    Environment.Exit 1
| Ok result ->
    let gridWidth =
        match result.WidthResult with
        | None ->
            Raylib.TraceLog(TraceLogLevel.Warning, $"No width value provided, using default: {Default.gridWidth}")
            Ok Default.gridWidth
        | Some result ->
            match result with
            | Error err ->
                match err with
                | NumberTooLarge value ->
                    Raylib.TraceLog(
                        TraceLogLevel.Error,
                        $"The width value was outside of the allowed range: {value}\n"
                        + $"Largest allowed value {Int32.MaxValue}"
                        + $"Smallest allowed value {Int32.MinValue}"
                    )

                    Error()

                | InvalidNumber value ->
                    Raylib.TraceLog(TraceLogLevel.Error, $"The provided width value was not a valid number: {value}")
                    Error()
                | NegativeNumber value ->
                    Raylib.TraceLog(TraceLogLevel.Error, $"The provided width value was negative: {value}")
                    Error()
                | NullInput ->
                    Raylib.TraceLog(TraceLogLevel.Error, $"The provided width string was null")
                    Error()

            | Ok value ->
                Raylib.TraceLog(TraceLogLevel.Info, $"Setting width = {value}")
                Ok value

    let gridHeight =
        match result.HeightResult with
        | None ->
            Raylib.TraceLog(TraceLogLevel.Warning, $"No height value provided, using default: {Default.gridHeight}")
            Ok Default.gridHeight
        | Some result ->
            match result with
            | Error err ->
                match err with
                | NumberTooLarge value ->
                    Raylib.TraceLog(
                        TraceLogLevel.Error,
                        $"The height value was outside of the allowed range: {value}\n"
                        + $"Largest allowed value {Int32.MaxValue}"
                        + $"Smallest allowed value {Int32.MinValue}"
                    )

                    Error()

                | InvalidNumber value ->
                    Raylib.TraceLog(TraceLogLevel.Error, $"The provided height value was not a valid number: {value}")
                    Error()
                | NegativeNumber value ->
                    Raylib.TraceLog(TraceLogLevel.Error, $"The provided height value was negative: {value}")
                    Error()
                | NullInput ->
                    Raylib.TraceLog(TraceLogLevel.Error, $"The provided height string was null")
                    Error()
            | Ok value ->
                Raylib.TraceLog(TraceLogLevel.Info, $"Setting height = {value}")
                Ok value

    match gridWidth, gridHeight with
    | Error(), _
    | _, Error() ->
        Raylib.TraceLog(TraceLogLevel.Error, $"Terminating the program")
        Environment.Exit 1
    | Ok width, Ok height ->
        let sleepTime = Default.sleepTimeCalculator width height
        let camera = new Camera(float32 (width / 2), float32 (height / 2))
        let startingState = ConwayGrid.createDead width height

        let canvas =
            new Canvas(
                Default.canvasX,
                Default.canvasY,
                float32 Default.windowWidth - Default.widthOffset,
                float32 Default.windowHeight - Default.heightOffset,
                camera,
                new Game(startingState),
                Default.cellSize
            )

        let controlManager = new ControlManager()

        let renderTexture =
            Raylib.LoadRenderTexture(Default.windowWidth, Default.windowHeight)

        let currentContext =
            new ApplicationContext(GameRunMode.Paused, canvas, renderTexture)

        controlManager.Buttons.AddRange(Buttons.instantiate currentContext)
        controlManager.KeyActions.AddRange(Hotkeys.mapKeyboardActions currentContext)
        controlManager.ShiftKeyActions.AddRange(Hotkeys.mapKeyboardShiftActions currentContext)

        let gameUpdateLoop () =
            let mutable shouldRun = false

            async {
                while true do
                    do! Async.Sleep sleepTime

                    shouldRun <-
                        match currentContext.GameMode with
                        | GameRunMode.Infinite -> true
                        | GameRunMode.Step ->
                            currentContext.GameMode <- GameRunMode.Paused
                            true
                        | GameRunMode.Paused
                        | _ -> false

                    if shouldRun then
                        canvas.Game.RunOneStep()
            }

        gameUpdateLoop () |> Async.Start

        let mutable fps = 0.0
        let maxSamples = Default.maxFpsSamples
        let frameTimes = Array.create maxSamples 0.0
        let mutable insertIndex = 0

        let stopwatch = Stopwatch.StartNew()

        while not (raylibTrue (Raylib.WindowShouldClose())) do
            let frameStart = stopwatch.Elapsed.TotalSeconds

            Display.mainWindow controlManager canvas renderTexture (int fps) (Raylib.GetMousePosition())

            controlManager.ReadInput()
            controlManager.UpdateControls()
            canvas.ProcessDrawableArea()

            let frameEnd = stopwatch.Elapsed.TotalSeconds
            let frameTime = frameEnd - frameStart

            frameTimes[insertIndex] <- frameTime
            insertIndex <- (insertIndex + 1) % maxSamples
            fps <- 1.0 / (frameTimes |> Array.average)

        Raylib.UnloadRenderTexture renderTexture
        Display.close ()
