open Conway.App
open Conway.App.Aliases
open Conway.App.File
open Conway.Core
open Conway.Encoding
open Raylib_cs
open System
open System.Diagnostics
open System.IO
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
    else if gridHeight <= 8000 || gridWidth <= 8000 then 8
    else 4

// 1 in 5 odds that a cell is living
let oddsOfGettingLivingCell = 5

let startingState =
    ConwayGrid.createRandomWithOdds gridWidth gridHeight oddsOfGettingLivingCell

let mutable game = new Game(startingState)

let mainLock = new ReaderWriterLockSlim()

let mutable gameRunningState = GameRunMode.Paused

let canvasX = 25.0f
let canvasY = 25.0f
let cellSize = 25.0f

let widthOffset = cellSize * 12.0f
let heightOffset = cellSize * 2.0f

let cameraPosX = 500.0f
let cameraPosY = 500.0f

let camera = new Camera(cameraPosX, cameraPosY)

let canvas =
    new Canvas(
        canvasX,
        canvasY,
        float32 windowWidth - widthOffset,
        float32 windowHeight - heightOffset,
        camera,
        game,
        cellSize
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

            game <-
                Game.createFrom
                    canvasWrapper.Game.CurrentState
                    canvasWrapper.Game.InitialState
                    canvasWrapper.Game.Generation

            canvas.Game <- game
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
        | GameRunMode.Paused -> game.RunOneStep()
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

    if button.IsActive && game.Generation <= 1 then
        button.IsActive <- false

let resetCallback () =
    try
        mainLock.EnterReadLock()

        match gameRunningState with
        | GameRunMode.Paused -> game.ResetState()
        | _ -> ()

    finally
        mainLock.ExitReadLock()

let clearCallback () =
    try
        mainLock.EnterReadLock()

        match gameRunningState with
        | GameRunMode.Paused -> game.CurrentState <- ConwayGrid.createDead gridWidth gridHeight
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
    |> Button.onClickCallback saveFile
    |> Button.onUpdateCallback updateOnRun

let loadButton =
    Button.create
    |> Button.position (windowWidth - 100) (windowHeight - 400)
    |> Button.size 50
    |> Button.text "Load"
    |> Button.onClickCallback openFile
    |> Button.onUpdateCallback updateOnRun

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
                    | GameRunMode.Infinite -> true
                    | GameRunMode.Step ->
                        gameRunningState <- GameRunMode.Paused
                        true
                    | GameRunMode.Paused
                    | _ -> false
                finally
                    mainLock.ExitWriteLock()

            if shouldRun then
                game.RunOneStep()
    }

gameUpdateLoop () |> Async.Start

let renderTexture = Raylib.LoadRenderTexture(windowWidth, windowWidth)
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
