open Conway.Core
open Conway.App.Raylib
open Raylib_cs
open System.Threading

let startingState = Preset.presetOne |> ConwayGrid.initFromPreset

let raylibTrue expr = expr |> Convert.CBoolToFsBool

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

let readKeyPresses () =
    if raylibTrue (Raylib.IsKeyPressed KeyboardKey.Space) then
        toggleGame ()

    if raylibTrue (Raylib.IsKeyPressed KeyboardKey.Right) then
        try
            mutex.WaitOne() |> ignore

            match gameRunningState with
            | Infinite
            | Limited _ -> ()
            | Paused -> game.runOneStep ()

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

let toggleButton = new Button(700, 500, 50, "", Some toggleGame, Some update)

let controls = new ControlManager()
controls.AddButton toggleButton

let readUserInput () =
    readKeyPresses ()

    controls.ReadInput (fun _ -> raylibTrue (Raylib.IsMouseButtonPressed MouseButton.Left)) (fun _ ->
        Raylib.GetMousePosition())

gameRunningState |> gameUpdateLoop |> Async.Start

Display.init ()

while not (raylibTrue (Raylib.WindowShouldClose())) do
    readUserInput ()
    controls.Update()
    Display.render game.State.Board controls

Display.close ()
