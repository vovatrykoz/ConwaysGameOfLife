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

let keysToProcess = [ Keyboard.readSpacePress, toggleGame; Keyboard.readRightArrowKey, advanceOnce ]

let update (button: Button) =
    try
        mutex.WaitOne() |> ignore

        match gameRunningState with
        | Paused -> button.Text <- "Run"
        | _ -> button.Text <- "Pause"
    finally
        mutex.ReleaseMutex()

let toggleButton =
    new Button(700, 400, 50, "", true, true, Some toggleGame, Some update)

let advanceButton =
    new Button(700, 500, 50, "Next", true, true, Some advanceOnce, None)

let controlManager = new ControlManager()
controlManager.AddButton toggleButton
controlManager.AddButton advanceButton

game.State.Board
|> Array2D.iteri (fun row col cellType ->
    match cellType with
    | BorderCell -> ()
    | PlayerCell _ ->
        let pressCallback =
            fun _ ->
                try
                    mutex.WaitOne() |> ignore

                    match game.State.Board[row, col] with
                    | BorderCell -> ()
                    | PlayerCell cell ->
                        match cell.Status with
                        | Dead -> game.State.Board[row, col] <- (PlayerCell Cell.living)
                        | Alive -> game.State.Board[row, col] <- (PlayerCell Cell.dead)
                finally
                    mutex.ReleaseMutex()

        let updateCallback (button: Button) =
            try
                mutex.WaitOne() |> ignore

                match gameRunningState with
                | Paused -> button.IsActive <- true
                | _ -> button.IsActive <- false
            finally
                mutex.ReleaseMutex()

        controlManager.AddButton(
            new Button(col * 25, row * 25, 25, "", true, false, Some pressCallback, Some updateCallback)
        ))

let lmbFunc = fun _ -> raylibTrue (Raylib.IsMouseButtonDown MouseButton.Left)

let lmbUp = fun _ -> raylibTrue (Raylib.IsMouseButtonReleased MouseButton.Left)

let mousePosFunc = fun _ -> Raylib.GetMousePosition()

gameRunningState |> gameUpdateLoop |> Async.Start

Display.init ()

while not (raylibTrue (Raylib.WindowShouldClose())) do
    controlManager.ReadUserInput keysToProcess lmbFunc lmbUp mousePosFunc
    controlManager.UpdateControls()
    Display.render game.State.Board controlManager

Display.close ()
