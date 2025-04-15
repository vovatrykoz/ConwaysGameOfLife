open Conway.Core
open Conway.App.Raylib
open Raylib_cs
open System.Threading

let startingState = Preset.presetOne |> ConwayGrid.initFromPreset

let game = new Game(startingState)

Display.init ()
Display.render game.State.Board

let mutable hasRunOnce = false

while not (Raylib.WindowShouldClose() |> Convert.CBoolToFsBool) do
    if not hasRunOnce then
        Thread.Sleep 1000
        hasRunOnce <- true
    else
        Thread.Sleep 500

    game.runOneStep ()
    Display.render game.State.Board

Display.close ()
