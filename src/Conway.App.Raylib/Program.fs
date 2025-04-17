open Conway.Core
open Conway.App.Raylib
open Raylib_cs

let startingState = Preset.presetOne |> ConwayGrid.initFromPreset

let game = new Game(startingState)

Display.init ()

let mutable gameRunningState = Infinite

let rec gameUpdateLoop state =

    async {
        do! Async.Sleep 500

        match gameRunningState with
        | Infinite -> game.runOneStep ()
        | Limited x when x > 1 ->
            game.runOneStep ()
            gameRunningState <- Limited(x - 1)
        | Limited _ ->
            game.runOneStep ()
            gameRunningState <- Paused
        | Paused -> ()

        return! gameUpdateLoop state
    }

gameRunningState |> gameUpdateLoop |> Async.Start

while not (Raylib.WindowShouldClose() |> Convert.CBoolToFsBool) do
    Display.render game.State.Board gameRunningState

Display.close ()
