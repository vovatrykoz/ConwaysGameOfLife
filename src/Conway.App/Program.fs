open Conway.App.Config
open Conway.App.Controls
open Conway.App.Graphics
open Conway.App.File
open Conway.App.Utils.Alias
open Conway.Core
open Conway.Encoding
open Raylib_cs
open System
open System.Diagnostics
open System.IO
open System.Threading

Display.init Default.windowWidth Default.windowHeight

Display.loadingScreen (float32 (Default.windowWidth / 2)) (float32 (Default.windowHeight / 2))

let args = Environment.GetCommandLineArgs()

let gridWidth =
    if Array.length args >= 2 then
        try
            let result = int args[1]
            Raylib.TraceLog(TraceLogLevel.Info, $"Setting grid width to: {result}")
            result
        with ex ->
            let exceptionString = ex.ToString().Replace("\n", "\n\t")
            Raylib.TraceLog(TraceLogLevel.Error, $"Could not parse the width value. Given: {args[1]}")
            Raylib.TraceLog(TraceLogLevel.Error, $"Detailed error information:\n\t{exceptionString}")
            Raylib.TraceLog(TraceLogLevel.Info, $"Setting the grid to the default width value: {Default.gridWidth}")
            Default.gridWidth
    else
        Raylib.TraceLog(
            TraceLogLevel.Info,
            $"No width value provided. Setting the grid to the default width value: {Default.gridWidth}"
        )

        Default.gridWidth

let gridHeight =
    if Array.length args = 3 then
        try
            let result = int args[2]
            Raylib.TraceLog(TraceLogLevel.Info, $"Setting grid height to: {result}")
            result
        with ex ->
            let exceptionString = ex.ToString().Replace("\n", "\n\t")
            Raylib.TraceLog(TraceLogLevel.Error, $"Could not parse the width value. Given: {args[2]}")
            Raylib.TraceLog(TraceLogLevel.Error, $"Detailed error information:\n\t{exceptionString}")
            Raylib.TraceLog(TraceLogLevel.Info, $"Setting the grid to the default height value: {Default.gridHeight}")

            Default.gridHeight
    else
        Raylib.TraceLog(
            TraceLogLevel.Info,
            $"No height value provided. Setting the grid to the default height value: {Default.gridHeight}"
        )

        Default.gridHeight

let sleepTime = Default.sleepTimeCalculator gridWidth gridHeight

let mainLock = new ReaderWriterLockSlim()

let mutable gameRunningState = GameRunMode.Paused

let camera = new Camera(Default.cameraPosX, Default.cameraPosY)

let canvas =
    new Canvas(
        Default.canvasX,
        Default.canvasY,
        float32 Default.windowWidth - Default.widthOffset,
        float32 Default.windowHeight - Default.heightOffset,
        camera,
        new Game(Default.startingState),
        Default.cellSize
    )

let controlManager = new ControlManager(canvas)

let saveFile () =
    try
        let saveFilesPath = Environment.CurrentDirectory + "/Saves"

        if not (Directory.Exists saveFilesPath) then
            Raylib.TraceLog(TraceLogLevel.Info, "Creating a save files directory...")
            Directory.CreateDirectory saveFilesPath |> ignore
            Raylib.TraceLog(TraceLogLevel.Info, "Save files directory created")

        let newFile = "./Saves/Test.cgol"
        let encoder = new ConwayByteEncoder()
        let fileSaver = new BinaryCanvasFileSaver(encoder :> IConwayByteEncoder)

        Raylib.TraceLog(TraceLogLevel.Info, $"Saving the file to {newFile} ...")
        let result = (fileSaver :> ICanvasFileSaver).Save canvas newFile

        match result with
        | Ok _ -> Raylib.TraceLog(TraceLogLevel.Info, "Test file saved successfully")
        | Error errorMessage ->
            Raylib.TraceLog(TraceLogLevel.Error, $"Could not save the file due to the following error: {errorMessage}")
    with ex ->
        let excepionMessage = ex.Message.ToString().Replace("\n", "\n\t")

        Raylib.TraceLog(
            TraceLogLevel.Error,
            $"Failed to create a directory with the following error: {excepionMessage}"
        )

let openFile () =
    try
        let saveFilesPath = Environment.CurrentDirectory + "/Saves"

        if not (Directory.Exists saveFilesPath) then
            Raylib.TraceLog(TraceLogLevel.Info, "Creating a save files directory...")
            Directory.CreateDirectory saveFilesPath |> ignore
            Raylib.TraceLog(TraceLogLevel.Info, "Save files directory created")

        let newFile = "./Saves/Test.cgol"
        let decoder = new ConwayByteDecoder()
        let fileSaver = new BinaryCanvasFileLoader(decoder :> IConwayByteDecoder)

        Raylib.TraceLog(TraceLogLevel.Info, $"Loading the file from {newFile} ...")
        let result = (fileSaver :> ICanvasFileLoader).Load newFile

        match result with
        | Ok canvasWrapper ->
            Raylib.TraceLog(TraceLogLevel.Info, "Test file loaded successfully")

            match canvasWrapper.OptionalMessage with
            | None -> ()
            | Some message -> Raylib.TraceLog(TraceLogLevel.Info, message)

            Raylib.TraceLog(TraceLogLevel.Info, "Updating the grid...")

            canvas.Game <-
                Game.createFrom
                    canvasWrapper.Game.CurrentState
                    canvasWrapper.Game.InitialState
                    canvasWrapper.Game.Generation

            canvas.Camera <- canvasWrapper.Camera
            Raylib.TraceLog(TraceLogLevel.Info, "Grid updated")
        | Error errorMessage ->
            Raylib.TraceLog(TraceLogLevel.Error, $"Could not load the file due to the following error: {errorMessage}")
    with ex ->
        let excepionMessage = ex.Message.ToString().Replace("\n", "\n\t")

        Raylib.TraceLog(
            TraceLogLevel.Error,
            $"Failed to create a directory with the following error: {excepionMessage}"
        )

let toggleGame () =
    try
        mainLock.EnterWriteLock()

        gameRunningState <-
            match gameRunningState with
            | GameRunMode.Paused -> GameRunMode.Infinite
            | _ -> GameRunMode.Paused
    finally
        mainLock.ExitWriteLock()

let advanceOnce () =
    try
        mainLock.EnterReadLock()

        match gameRunningState with
        | GameRunMode.Paused -> canvas.Game.RunOneStep()
        | _ -> ()
    finally
        mainLock.ExitReadLock()

let update (button: Button) =
    try
        mainLock.EnterReadLock()

        match gameRunningState with
        | GameRunMode.Paused -> button.Text <- "Run"
        | _ -> button.Text <- "Pause"
    finally
        mainLock.ExitReadLock()

let updateOnRun (button: Button) =
    try
        mainLock.EnterReadLock()

        match gameRunningState with
        | GameRunMode.Paused -> button.IsActive <- true
        | _ -> button.IsActive <- false
    finally
        mainLock.ExitReadLock()

let updateOnRunBack (button: Button) =
    updateOnRun button

    if button.IsActive && canvas.Game.Generation <= 1 then
        button.IsActive <- false

let resetCallback () =
    try
        mainLock.EnterReadLock()

        match gameRunningState with
        | GameRunMode.Paused -> canvas.Game.ResetState()
        | _ -> ()

    finally
        mainLock.ExitReadLock()

let clearCallback () =
    try
        mainLock.EnterReadLock()

        match gameRunningState with
        | GameRunMode.Paused -> canvas.Game.CurrentState <- ConwayGrid.createDead gridWidth gridHeight
        | _ -> ()

    finally
        mainLock.ExitReadLock()

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

let saveButton =
    Button.create
    |> Button.position (Default.windowWidth - 200) (Default.windowHeight - 400)
    |> Button.size 50
    |> Button.text "Save"
    |> Button.onClickCallback saveFile
    |> Button.onUpdateCallback updateOnRun

let loadButton =
    Button.create
    |> Button.position (Default.windowWidth - 100) (Default.windowHeight - 400)
    |> Button.size 50
    |> Button.text "Load"
    |> Button.onClickCallback openFile
    |> Button.onUpdateCallback updateOnRun

let runButton =
    Button.create
    |> Button.position (Default.windowWidth - 200) (Default.windowHeight - 200)
    |> Button.size 50
    |> Button.onClickCallback toggleGame
    |> Button.onUpdateCallback update
    |> Button.shortcut KeyboardKey.Space

let advanceButton =
    Button.create
    |> Button.position (Default.windowWidth - 100) (Default.windowHeight - 200)
    |> Button.size 50
    |> Button.text "Next"
    |> Button.onClickCallback advanceOnce
    |> Button.onUpdateCallback updateOnRun
    |> Button.shortcut KeyboardKey.Right

let resetButton =
    Button.create
    |> Button.position (Default.windowWidth - 100) (Default.windowHeight - 100)
    |> Button.size 50
    |> Button.text "Reset"
    |> Button.onClickCallback resetCallback
    |> Button.onUpdateCallback updateOnRun

let clearButton =
    Button.create
    |> Button.position (Default.windowWidth - 200) (Default.windowHeight - 100)
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
                    | GameRunMode.Infinite -> true
                    | GameRunMode.Step ->
                        gameRunningState <- GameRunMode.Paused
                        true
                    | GameRunMode.Paused
                    | _ -> false
                finally
                    mainLock.ExitWriteLock()

            if shouldRun then
                canvas.Game.RunOneStep()
    }

gameUpdateLoop () |> Async.Start

let renderTexture =
    Raylib.LoadRenderTexture(Default.windowWidth, Default.windowHeight)

let mutable fps = 0.0
let maxSamples = Default.maxFpsSamples
let frameTimes = Array.create maxSamples 0.0
let mutable insertIndex = 0

let stopwatch = Stopwatch.StartNew()

while not (raylibTrue (Raylib.WindowShouldClose())) do
    let frameStart = stopwatch.Elapsed.TotalSeconds

    controlManager.ReadInput()
    controlManager.UpdateControls()
    Display.render controlManager renderTexture (int fps) (Raylib.GetMousePosition())

    let frameEnd = stopwatch.Elapsed.TotalSeconds
    let frameTime = frameEnd - frameStart

    frameTimes[insertIndex] <- frameTime
    insertIndex <- (insertIndex + 1) % maxSamples
    fps <- 1.0 / (frameTimes |> Array.average)

Raylib.UnloadRenderTexture renderTexture
Display.close ()
