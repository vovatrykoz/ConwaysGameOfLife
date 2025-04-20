open Conway.Core
open Conway.App.Raylib
open Conway.App.Raylib.Aliases
open Raylib_cs
open System.Threading

let startingState = Preset.presetOne |> ConwayGrid.initFromPreset

let game = new Game(startingState)

let mutex = new Mutex()

let mutable gameRunningState = Paused

let rec gameUpdateLoop state =
    async {
        do! Async.Sleep 500

        try
            mutex.WaitOne() |> ignore

            match gameRunningState with
            | Infinite -> game.runOneStep ()
            | Limited x when x > 1 ->
                game.runOneStep ()
                gameRunningState <- Limited(x - 1)
            | Limited _ ->
                game.runOneStep ()
                gameRunningState <- Paused
            | Paused -> ()
        finally
            mutex.ReleaseMutex()

        return! gameUpdateLoop state
    }

let toggleGame () =
    try
        mutex.WaitOne() |> ignore

        gameRunningState <-
            match gameRunningState with
            | Paused -> Infinite
            | _ -> Paused
    finally
        mutex.ReleaseMutex()

let advanceOnce () =
    try
        mutex.WaitOne() |> ignore

        match gameRunningState with
        | Infinite
        | Limited _ -> ()
        | Paused -> game.runOneStep ()

    finally
        mutex.ReleaseMutex()

let advanceBackOnce () =
    try
        mutex.WaitOne() |> ignore

        match gameRunningState with
        | Infinite
        | Limited _ -> ()
        | Paused -> game.stepBack ()

    finally
        mutex.ReleaseMutex()

let update (button: Button) =
    try
        mutex.WaitOne() |> ignore

        match gameRunningState with
        | Paused -> button.Text <- "Run"
        | _ -> button.Text <- "Pause"
    finally
        mutex.ReleaseMutex()

let updateOnRun (button: Button) =
    try
        mutex.WaitOne() |> ignore

        match gameRunningState with
        | Paused -> button.IsActive <- true
        | _ -> button.IsActive <- false
    finally
        mutex.ReleaseMutex()

let updateOnRunBack (button: Button) =
    updateOnRun button

    if button.IsActive && game.hasMemoryLoss () then
        button.IsActive <- false

let resetCallback () =
    try
        mutex.WaitOne() |> ignore

        match gameRunningState with
        | Infinite
        | Limited _ -> ()
        | Paused -> game.State <- startingState

        game.clearHistory ()

    finally
        mutex.ReleaseMutex()

let clearCallback () =
    try
        mutex.WaitOne() |> ignore

        match gameRunningState with
        | Infinite
        | Limited _ -> ()
        | Paused -> game.State <- Preset.deadPreset |> ConwayGrid.initFromPreset

        game.clearHistory ()
    finally
        mutex.ReleaseMutex()

let toggleButton =
    Button.create
    |> Button.position 600 300
    |> Button.size 50
    |> Button.onClickCallback toggleGame
    |> Button.onUpdateCallback update
    |> Button.shortcut KeyboardKey.Space

let advanceButton =
    Button.create
    |> Button.position 700 400
    |> Button.size 50
    |> Button.text "Next"
    |> Button.onClickCallback advanceOnce
    |> Button.onUpdateCallback updateOnRun
    |> Button.shortcut KeyboardKey.Right

let advanceBackButton =
    Button.create
    |> Button.position 600 400
    |> Button.size 50
    |> Button.text "Previous"
    |> Button.onClickCallback advanceBackOnce
    |> Button.onUpdateCallback updateOnRunBack
    |> Button.shortcut KeyboardKey.Left

let resetButton =
    Button.create
    |> Button.position 700 500
    |> Button.size 50
    |> Button.text "Reset"
    |> Button.onClickCallback resetCallback
    |> Button.onUpdateCallback updateOnRun

let clearButton =
    Button.create
    |> Button.position 600 500
    |> Button.size 50
    |> Button.text "Clear"
    |> Button.onClickCallback clearCallback
    |> Button.onUpdateCallback updateOnRun

let buttons = [ toggleButton; advanceButton; advanceBackButton; resetButton; clearButton ]

let controlManager = new ControlManager(game)
controlManager.AddButtons buttons

let keyboardActions = [|
    KeyboardKey.W, controlManager.Canvas.MoveCameraUp
    KeyboardKey.A, controlManager.Canvas.MoveCameraLeft
    KeyboardKey.S, controlManager.Canvas.MoveCameraDown
    KeyboardKey.D, controlManager.Canvas.MoveCameraRight
|]

controlManager.KeyActions <- keyboardActions

gameRunningState |> gameUpdateLoop |> Async.Start

Display.init ()

while not (raylibTrue (Raylib.WindowShouldClose())) do
    controlManager.ReadInput()
    controlManager.UpdateControls()
    Display.render game controlManager

Display.close ()
