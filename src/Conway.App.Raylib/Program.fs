open Conway.Core
open Conway.App.Raylib
open Conway.App.Raylib.Aliases
open Raylib_cs
open System
open System.Diagnostics
open System.Threading

let windowWidth = 1920

let windowHeight = 1080

Display.init windowWidth windowHeight

Display.loadingScreen (float32 (windowWidth / 2)) (float32 (windowHeight / 2))

let defaultGridWidth = 1000
let defaultGridHeight = 1000

let args = Environment.GetCommandLineArgs()

let gridWidth =
    if Array.length args >= 2 then
        try
            let result = int args[1]
            Raylib.TraceLog(TraceLogLevel.Info, $"Setting grid width to: {result}")
            result
        with _ ->
            Raylib.TraceLog(TraceLogLevel.Error, $"Could not parse the width value. Given: {args[1]}")
            Raylib.TraceLog(TraceLogLevel.Info, $"Setting the grid to the default width value: {defaultGridWidth}")
            defaultGridWidth
    else
        Raylib.TraceLog(
            TraceLogLevel.Info,
            $"No width value provided. Setting the grid to the default width value: {defaultGridWidth}"
        )

        defaultGridWidth

let gridHeight =
    if Array.length args = 3 then
        try
            let result = int args[2]
            Raylib.TraceLog(TraceLogLevel.Info, $"Setting grid height to: {result}")
            result
        with _ ->
            Raylib.TraceLog(TraceLogLevel.Error, $"Could not parse the width value. Given: {args[2]}")
            Raylib.TraceLog(TraceLogLevel.Info, $"Setting the grid to the default height value: {defaultGridHeight}")

            defaultGridHeight
    else
        Raylib.TraceLog(
            TraceLogLevel.Info,
            $"No height value provided. Setting the grid to the default height value: {defaultGridHeight}"
        )

        defaultGridHeight

// 1 in 5 odds that a cell is living
let oddsOfGettingLivingCell = 5

let startingState =
    ConwayGrid.createRandomWithOdds gridWidth gridHeight oddsOfGettingLivingCell

let game = new Game(startingState)

let mainLock = new ReaderWriterLockSlim()

let mutable gameRunningState = Paused

let gameUpdateLoop () =
    let mutable shouldRun = false

    async {
        while true do
            do! Async.Sleep 34

            try
                mainLock.EnterWriteLock()

                shouldRun <-
                    match gameRunningState with
                    | Infinite -> true
                    | Limited x when x > 1 ->
                        gameRunningState <- Limited(x - 1)
                        true
                    | Limited _ ->
                        gameRunningState <- Paused
                        true
                    | Paused -> false
            finally
                mainLock.ExitWriteLock()

            if shouldRun then
                game.RunOneStep()
    }

let toggleGame () =
    try
        mainLock.EnterWriteLock()

        gameRunningState <-
            match gameRunningState with
            | Paused -> Infinite
            | _ -> Paused
    finally
        mainLock.ExitWriteLock()

let advanceOnce () =
    try
        mainLock.EnterReadLock()

        match gameRunningState with
        | Infinite
        | Limited _ -> ()
        | Paused -> game.RunOneStep()
    finally
        mainLock.ExitReadLock()

let update (button: Button) =
    try
        mainLock.EnterReadLock()

        match gameRunningState with
        | Paused -> button.Text <- "Run"
        | _ -> button.Text <- "Pause"
    finally
        mainLock.ExitReadLock()

let updateOnRun (button: Button) =
    try
        mainLock.EnterReadLock()

        match gameRunningState with
        | Paused -> button.IsActive <- true
        | _ -> button.IsActive <- false
    finally
        mainLock.ExitReadLock()

let updateOnRunBack (button: Button) =
    updateOnRun button

    if button.IsActive && game.Generation <= 1 then
        button.IsActive <- false

let resetCallback () =
    try
        mainLock.EnterReadLock()

        match gameRunningState with
        | Infinite
        | Limited _ -> ()
        | Paused -> game.State <- ConwayGrid.createDead gridWidth gridHeight

        game.ClearHistory()
    finally
        mainLock.ExitReadLock()

let clearCallback () =
    try
        mainLock.EnterReadLock()

        match gameRunningState with
        | Infinite
        | Limited _ -> ()
        | Paused -> game.State <- ConwayGrid.createDead gridWidth gridHeight

        game.ClearHistory()
    finally
        mainLock.ExitReadLock()

let toggleButton =
    Button.create
    |> Button.position (windowWidth - 200) (windowHeight - 300)
    |> Button.size 50
    |> Button.onClickCallback toggleGame
    |> Button.onUpdateCallback update
    |> Button.shortcut KeyboardKey.Space

let advanceButton =
    Button.create
    |> Button.position (windowWidth - 100) (windowHeight - 200)
    |> Button.size 50
    |> Button.text "Next"
    |> Button.onClickCallback advanceOnce
    |> Button.onUpdateCallback updateOnRun
    |> Button.shortcut KeyboardKey.Right

let resetButton =
    Button.create
    |> Button.position (windowWidth - 100) (windowHeight - 100)
    |> Button.size 50
    |> Button.text "Reset"
    |> Button.onClickCallback resetCallback
    |> Button.onUpdateCallback updateOnRun

let clearButton =
    Button.create
    |> Button.position (windowWidth - 200) (windowHeight - 100)
    |> Button.size 50
    |> Button.text "Clear"
    |> Button.onClickCallback clearCallback
    |> Button.onUpdateCallback updateOnRun

let buttons = [| toggleButton; advanceButton; resetButton; clearButton |]

let canvasX = 25.0f
let canvasY = 25.0f
let cellSize = 25.0f

let widthOffset = cellSize * 12.0f
let heightOffset = cellSize * 2.0f

let focusAreaX = 500.0f
let focusAreaY = 500.0f

let scale = 1

let canvas =
    new Canvas(
        canvasX,
        canvasY,
        float32 windowWidth - widthOffset,
        float32 windowHeight - heightOffset,
        focusAreaX,
        focusAreaY,
        game,
        cellSize,
        scale
    )

let controlManager = new ControlManager(canvas)
controlManager.Buttons.AddRange buttons

let keyboardActions = [|
    KeyboardKey.W, (fun _ -> controlManager.Canvas.MoveCameraUp 1.0f)
    KeyboardKey.A, (fun _ -> controlManager.Canvas.MoveCameraLeft 1.0f)
    KeyboardKey.S, (fun _ -> controlManager.Canvas.MoveCameraDown 1.0f)
    KeyboardKey.D, (fun _ -> controlManager.Canvas.MoveCameraRight 1.0f)
    KeyboardKey.Z, (fun _ -> controlManager.Canvas.ZoomIn 5.0f)
    KeyboardKey.X, (fun _ -> controlManager.Canvas.ZoomOut 5.0f)
|]

controlManager.KeyActions.AddRange keyboardActions

gameUpdateLoop () |> Async.Start

let renderTexture = Raylib.LoadRenderTexture(int canvas.Width, int canvas.Height)
let mutable fps = 0.0
let maxSamples = 60
let frameTimes = Array.create maxSamples 0.0
let mutable insertIndex = 0

let stopwatch = Stopwatch.StartNew()

while not (raylibTrue (Raylib.WindowShouldClose())) do
    let frameStart = stopwatch.Elapsed.TotalSeconds

    controlManager.ReadInput()
    controlManager.UpdateControls()
    Display.render game controlManager renderTexture (int fps)

    let frameEnd = stopwatch.Elapsed.TotalSeconds
    let frameTime = frameEnd - frameStart

    frameTimes[insertIndex] <- frameTime
    insertIndex <- (insertIndex + 1) % maxSamples

    fps <- 1.0 / (frameTimes |> Array.average)

Raylib.UnloadRenderTexture renderTexture
Display.close ()
