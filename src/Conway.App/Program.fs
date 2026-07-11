open Conway.App
open Conway.App.Config
open Conway.App.Controls
open Conway.App.Graphics
open Conway.App.Math
open Conway.App.Utils.Alias
open Conway.Core
open Raylib_cs
open System
open System.Diagnostics

Display.init Default.windowWidth Default.windowHeight

Display.loadingScreen
    (LanguagePrimitives.Float32WithMeasure<px>(float32 (Default.windowWidth / 2)))
    (LanguagePrimitives.Float32WithMeasure<px>(float32 (Default.windowHeight / 2)))

let args = Environment.GetCommandLineArgs()

let userInput = UserInput.tryReadArgs args

match UserInput.tryReadArgs args with
| Error err ->
    Log.userInputError err
    Environment.Exit 1

| Ok result ->
    let gridWidth = Resolve.dimension "width" Default.gridWidth result.WidthResult
    let gridHeight = Resolve.dimension "height" Default.gridHeight result.HeightResult

    match gridWidth, gridHeight with
    | Error(), _
    | _, Error() ->
        Raylib.TraceLog(TraceLogLevel.Error, "Terminating the program!")
        Environment.Exit 1

    | Ok width, Ok height ->
        let sleepTime = Default.sleepTimeCalculator width height

        let startingCameraPosX: float32<cells> =
            LanguagePrimitives.Float32WithMeasure<cells>(float32 (width / 2))

        let startingCameraPosY: float32<cells> =
            LanguagePrimitives.Float32WithMeasure<cells>(float32 (height / 2))

        let camera = new Camera<cells>(x = startingCameraPosX, y = startingCameraPosY)

        let startingState = ConwayGrid.createDead (int width) (int height)

        let canvas =
            new Canvas(
                x = Default.canvasX,
                y = Default.canvasY,
                width = Default.canvasWidth,
                height = Default.canvasHeight,
                camera = camera,
                game = new Game(startingState),
                cellSize = Default.cellSize
            )

        let controlManager = new ControlManager()

        let renderTexture =
            Raylib.LoadRenderTexture(int Default.windowWidth, int Default.windowHeight)

        let appContext =
            new ApplicationContext(
                gameMode = GameState.Paused,
                canvas = canvas,
                texture = renderTexture,
                sleepTime = sleepTime
            )

        controlManager.Buttons.AddRange(Buttons.instantiate appContext)
        controlManager.KeyActions.AddRange(Hotkeys.mapKeyboardActions appContext)
        controlManager.ShiftKeyActions.AddRange(Hotkeys.mapKeyboardShiftActions appContext)
        controlManager.CtrlKeyActions.AddRange(Hotkeys.mapKeyboardCtrlActions appContext)

        let gameUpdateLoop (ctx: ApplicationContext) =
            task {
                while true do
                    do! Async.Sleep ctx.SleepTime

                    match appContext.GameMode with
                    | GameState.Infinite -> canvas.Game.RunOneStep()
                    | GameState.Step ->
                        canvas.Game.RunOneStep()
                        appContext.GameMode <- GameState.Paused
                    | GameState.Paused
                    | _ -> ()
            }

        gameUpdateLoop appContext |> ignore

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
