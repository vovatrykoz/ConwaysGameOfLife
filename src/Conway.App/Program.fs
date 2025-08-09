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

let gridWidth =
    UserInput.tryReadArg args Default.gridWidthIndex "width" Default.gridWidth

let gridHeight =
    UserInput.tryReadArg args Default.gridHeightIndex "height" Default.gridHeight

let sleepTime = Default.sleepTimeCalculator gridWidth gridHeight

let camera = new Camera(float32 (gridWidth / 2), float32 (gridHeight / 2))

let startingState =
    ConwayGrid.createRandomWithOdds gridWidth gridHeight Default.oddsOfGettingLivingCell

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
