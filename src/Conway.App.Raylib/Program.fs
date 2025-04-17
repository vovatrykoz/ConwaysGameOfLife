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

let readMouseClicks () =
    if raylibTrue (Raylib.IsMouseButtonPressed MouseButton.Left) then
        let mousePos = Raylib.GetMousePosition()

        if
            mousePos.X >= 700.0f
            && mousePos.X <= 750.0f
            && mousePos.Y >= 500.0f
            && mousePos.Y <= 550.0f
        then
            toggleGame ()

let readUserInput () =
    readKeyPresses ()
    readMouseClicks ()

gameRunningState |> gameUpdateLoop |> Async.Start

Display.init ()

while not (raylibTrue (Raylib.WindowShouldClose())) do
    readUserInput ()
    Display.render game.State.Board gameRunningState

Display.close ()
