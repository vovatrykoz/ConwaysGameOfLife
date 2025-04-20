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

let keysToProcess = []

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
    new Button(600, 300, 50, "", true, true, Some toggleGame, None, Some update, seq { KeyboardKey.Space })

let advanceButton =
    new Button(700, 400, 50, "Next", true, true, Some advanceOnce, None, Some updateOnRun, seq { KeyboardKey.Right })

let advanceBackButton =
    new Button(
        600,
        400,
        50,
        "Previous",
        true,
        true,
        Some advanceBackOnce,
        None,
        Some updateOnRunBack,
        seq { KeyboardKey.Left }
    )

let resetButton =
    new Button(700, 500, 50, "Reset", true, true, Some resetCallback, None, None, Seq.empty)

let clearButton =
    new Button(600, 500, 50, "Clear", true, true, Some clearCallback, None, None, Seq.empty)

let controlManager = new ControlManager()
controlManager.AddButton toggleButton
controlManager.AddButton advanceButton
controlManager.AddButton advanceBackButton
controlManager.AddButton resetButton
controlManager.AddButton clearButton

game.State.Board
|> Array2D.iteri (fun row col cellType ->
    match cellType with
    | BorderCell -> ()
    | PlayerCell _ ->
        let makeAliveCallback =
            fun _ ->
                match game.State.Board[row, col] with
                | BorderCell -> ()
                | PlayerCell _ -> game.State.Board[row, col] <- (PlayerCell Cell.living)

                // erase the history since the player has altered the board
                game.clearHistory ()

        let makeDeadCallback =
            fun _ ->
                match game.State.Board[row, col] with
                | BorderCell -> ()
                | PlayerCell _ -> game.State.Board[row, col] <- (PlayerCell Cell.dead)

                // erase the history since the player has altered the board
                game.clearHistory ()

        controlManager.AddGridControl(
            new GridControl(col * 25, row * 25, 25, Some makeAliveCallback, Some makeDeadCallback)
        ))

gameRunningState |> gameUpdateLoop |> Async.Start

Display.init ()

while not (raylibTrue (Raylib.WindowShouldClose())) do
    controlManager.ReadUserInput keysToProcess
    controlManager.UpdateControls()
    Display.render game controlManager

Display.close ()
