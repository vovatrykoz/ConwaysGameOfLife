
open Conway.Core
open Conway.App
open Conway.App.Aliases
open Raylib_cs
open System
open System.Diagnostics
open System.Threading

let windowWidth = 1024

let windowHeight = 768

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

let sleepTime =
    if gridHeight <= 2000 || gridWidth <= 2000 then 34
    else if gridHeight <= 5000 || gridWidth <= 5000 then 16
    else 0

// 1 in 5 odds that a cell is living
let oddsOfGettingLivingCell = 5

let startingState =
    ConwayGrid.createRandomWithOdds gridWidth gridHeight oddsOfGettingLivingCell

let game = new Game(startingState)

let mainLock = new ReaderWriterLockSlim()

let mutable gameRunningState = Paused

let canvasX = 25.0f
let canvasY = 25.0f
let cellSize = 25.0f

let widthOffset = cellSize * 12.0f
let heightOffset = cellSize * 2.0f

let cameraPosX = 500.0f
let cameraPosY = 500.0f

let canvas =
    new Canvas(
        canvasX,
        canvasY,
        float32 windowWidth - widthOffset,
        float32 windowHeight - heightOffset,
        cameraPosX,
        cameraPosY,
        game,
        cellSize
    )

let controlManager = new ControlManager(canvas)

let openFile () =
    Gtk.Application.Init()

    let dialog =
        new Gtk.FileChooserDialog(
            "Open File",
            null,
            Gtk.FileChooserAction.Open,
            [| "Cancel", Gtk.ResponseType.Cancel; "Open", Gtk.ResponseType.Accept |]
        )

    dialog.SetDefaultSize(800, 600)

    let response = dialog.Run()

    if response = int Gtk.ResponseType.Accept then
        let filename = dialog.Filename
        printfn "Selected file: %s" filename
    else
        printfn "No file selected."

    dialog.Destroy()
    Gtk.Application.Quit()

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
        | Paused -> game.RunOneStep()
        | _ -> ()
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
        | Paused -> game.ResetState()
        | _ -> ()

    finally
        mainLock.ExitReadLock()

let clearCallback () =
    try
        mainLock.EnterReadLock()

        match gameRunningState with
        | Paused -> game.CurrentState <- ConwayGrid.createDead gridWidth gridHeight
        | _ -> ()

    finally
        mainLock.ExitReadLock()

let fullscreenUpdate () =
    if raylibTrue (Raylib.IsKeyPressed KeyboardKey.Y) then
        if raylibTrue (Raylib.IsWindowFullscreen()) then
            Raylib.SetWindowSize(windowWidth, windowHeight)
        else
            let monitor = Raylib.GetCurrentMonitor()
            let monitorWidth = Raylib.GetMonitorWidth monitor
            let monitorHeight = Raylib.GetMonitorHeight monitor
            Raylib.SetWindowSize(monitorWidth, monitorHeight)

        Raylib.ToggleFullscreen()

let saveButton =
    Button.create
    |> Button.position (windowWidth - 200) (windowHeight - 400)
    |> Button.size 50
    |> Button.text "Save"

let loadButton =
    Button.create
    |> Button.position (windowWidth - 100) (windowHeight - 400)
    |> Button.size 50
    |> Button.text "Load"
    |> Button.onClickCallback openFile

let runButton =
    Button.create
    |> Button.position (windowWidth - 200) (windowHeight - 200)
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

let buttons = [| runButton; advanceButton; resetButton; clearButton; saveButton; loadButton |]
controlManager.Buttons.AddRange buttons

let keyboardActions = [|
    KeyboardKey.W, (fun _ -> controlManager.Canvas.Camera.MoveCameraUp 1.0f)
    KeyboardKey.A, (fun _ -> controlManager.Canvas.Camera.MoveCameraLeft 1.0f)
    KeyboardKey.S, (fun _ -> controlManager.Canvas.Camera.MoveCameraDown 1.0f)
    KeyboardKey.D, (fun _ -> controlManager.Canvas.Camera.MoveCameraRight 1.0f)
    KeyboardKey.Z, (fun _ -> controlManager.Canvas.Camera.ZoomIn 0.2f)
    KeyboardKey.X, (fun _ -> controlManager.Canvas.Camera.ZoomOut 0.2f)
|]

let keyboardShiftActions = [|
    KeyboardKey.W, (fun _ -> controlManager.Canvas.Camera.MoveCameraUp 5.0f)
    KeyboardKey.A, (fun _ -> controlManager.Canvas.Camera.MoveCameraLeft 5.0f)
    KeyboardKey.S, (fun _ -> controlManager.Canvas.Camera.MoveCameraDown 5.0f)
    KeyboardKey.D, (fun _ -> controlManager.Canvas.Camera.MoveCameraRight 5.0f)
|]

controlManager.KeyActions.AddRange keyboardActions
controlManager.ShiftKeyActions.AddRange keyboardShiftActions

let gameUpdateLoop () =
    let mutable shouldRun = false

    async {
        while true do
            do! Async.Sleep sleepTime

            shouldRun <-
                try
                    mainLock.EnterWriteLock()

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
    Display.render game controlManager renderTexture (int fps) (Raylib.GetMousePosition())

    let frameEnd = stopwatch.Elapsed.TotalSeconds
    let frameTime = frameEnd - frameStart

    frameTimes[insertIndex] <- frameTime
    insertIndex <- (insertIndex + 1) % maxSamples
    fps <- 1.0 / (frameTimes |> Array.average)

Raylib.UnloadRenderTexture renderTexture
Display.close ()
