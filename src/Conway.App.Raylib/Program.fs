open Conway.Core
open Conway.App.Raylib
open Conway.App.Raylib.Aliases
open Raylib_cs
open System

let defaultGridWidth = 1001
let defaultGridHeight = 1001

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

let windowWidth = 1920

let windowHeight = 1080

// 1 in 5 odds that a cell is living
let oddsOfGettingLivingCell = 5

let startingState =
    ConwayGrid.createRandomWithOdds gridWidth gridHeight oddsOfGettingLivingCell

let game = new Game(startingState)

let mainSync = obj ()

let withLock f = lock mainSync f

let mutable gameRunningState = Paused

let rec gameUpdateLoop state =
    async {
        do! Async.Sleep 34

        withLock (fun _ ->
            match gameRunningState with
            | Infinite -> game.runOneStep ()
            | Limited x when x > 1 ->
                game.runOneStep ()
                gameRunningState <- Limited(x - 1)
            | Limited _ ->
                game.runOneStep ()
                gameRunningState <- Paused
            | Paused -> ())

        return! gameUpdateLoop state
    }

let toggleGame () =
    withLock (fun _ ->
        gameRunningState <-
            match gameRunningState with
            | Paused -> Infinite
            | _ -> Paused)

let advanceOnce () =
    withLock (fun _ ->
        match gameRunningState with
        | Infinite
        | Limited _ -> ()
        | Paused -> game.runOneStep ())

let advanceBackOnce () =
    withLock (fun _ ->
        match gameRunningState with
        | Infinite
        | Limited _ -> ()
        | Paused -> game.stepBack ())

let update (button: Button) =
    withLock (fun _ ->
        match gameRunningState with
        | Paused -> button.Text <- "Run"
        | _ -> button.Text <- "Pause")

let updateOnRun (button: Button) =
    withLock (fun _ ->
        match gameRunningState with
        | Paused -> button.IsActive <- true
        | _ -> button.IsActive <- false)

let updateOnRunBack (button: Button) =
    updateOnRun button

    if button.IsActive && game.Generation <= 1 then
        button.IsActive <- false

let resetCallback () =
    withLock (fun _ ->
        match gameRunningState with
        | Infinite
        | Limited _ -> ()
        | Paused -> game.State <- ConwayGrid.createDead gridWidth gridHeight

        game.clearHistory ())

let clearCallback () =
    withLock (fun _ ->
        match gameRunningState with
        | Infinite
        | Limited _ -> ()
        | Paused -> game.State <- ConwayGrid.createDead gridWidth gridHeight

        game.clearHistory ())

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

let advanceBackButton =
    Button.create
    |> Button.position (windowWidth - 200) (windowHeight - 200)
    |> Button.size 50
    |> Button.text "Previous"
    |> Button.onClickCallback advanceBackOnce
    |> Button.onUpdateCallback updateOnRunBack
    |> Button.shortcut KeyboardKey.Left

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

let canvasX = 25
let canvasY = 25
let cellSize = 25

let widthOffset = cellSize * 12
let heightOffset = cellSize * 2

let focusAreaX = 500
let focusAreaY = 500

let scale = 1

let canvas =
    new Canvas(
        canvasX,
        canvasY,
        windowWidth - widthOffset,
        windowHeight - heightOffset,
        focusAreaX,
        focusAreaY,
        game,
        cellSize,
        scale
    )

let controlManager = new ControlManager(canvas)
controlManager.Buttons.AddRange buttons

let keyboardActions = [|
    KeyboardKey.W, controlManager.Canvas.MoveCameraUp
    KeyboardKey.A, controlManager.Canvas.MoveCameraLeft
    KeyboardKey.S, controlManager.Canvas.MoveCameraDown
    KeyboardKey.D, controlManager.Canvas.MoveCameraRight
    KeyboardKey.Z, controlManager.Canvas.ZoomIn
    KeyboardKey.X, controlManager.Canvas.ZoomOut
|]

controlManager.KeyActions.AddRange keyboardActions

gameRunningState |> gameUpdateLoop |> Async.Start

Display.init windowWidth windowHeight

let renderTexture = Raylib.LoadRenderTexture(canvas.Width, canvas.Height)

while not (raylibTrue (Raylib.WindowShouldClose())) do
    controlManager.ReadInput()
    controlManager.UpdateControls()
    Display.render game controlManager renderTexture

Raylib.UnloadRenderTexture renderTexture
Display.close ()
