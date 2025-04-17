open Conway.Core
open Conway.App.Raylib
open Raylib_cs
open System.Threading

let startingState = Preset.presetOne |> ConwayGrid.initFromPreset

let raylibTrue expr = expr |> Convert.CBoolToFsBool

let game = new Game(startingState)

Display.init ()

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

let readUserInput () =
    if raylibTrue (Raylib.IsKeyPressed KeyboardKey.Space) then
        try
            mutex.WaitOne() |> ignore

            gameRunningState <-
                match gameRunningState with
                | Paused -> Infinite
                | _ -> Paused
        finally
            mutex.ReleaseMutex()

    if raylibTrue (Raylib.IsKeyPressed KeyboardKey.Right) then
        try
            mutex.WaitOne() |> ignore

            match gameRunningState with
            | Infinite
            | Limited _ -> ()
            | Paused -> game.runOneStep ()

        finally
            mutex.ReleaseMutex()

gameRunningState |> gameUpdateLoop |> Async.Start

while not (raylibTrue (Raylib.WindowShouldClose())) do
    readUserInput ()
    Display.render game.State.Board gameRunningState

Display.close ()
