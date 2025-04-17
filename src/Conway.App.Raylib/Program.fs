open Conway.Core
open Conway.App.Raylib
open Raylib_cs

type GameState =
    | Running
    | Paused

let startingState = Preset.presetOne |> ConwayGrid.initFromPreset

let game = new Game(startingState)

Display.init ()
Display.render game.State.Board

let mutable hasRunOnce = false

let mutable currentState = Running

let rec gameUpdateLoop () =

    async {
        if not hasRunOnce then
            do! Async.Sleep 1000
            hasRunOnce <- true
        else
            do! Async.Sleep 500

        match currentState with
        | Running -> game.runOneStep ()
        | Paused -> ()

        return! gameUpdateLoop ()
    }

gameUpdateLoop () |> Async.Start

while not (Raylib.WindowShouldClose() |> Convert.CBoolToFsBool) do
    Display.render game.State.Board

Display.close ()
