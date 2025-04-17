namespace Conway.App.Raylib

open Conway.Core
open Raylib_cs

module Display =
    let init () =
        Raylib.InitWindow(800, 600, "Conway's game of life")

    let private renderBoard board =
        board
        |> Array2D.iteri (fun row col cell ->
            match cell with
            | BorderCell -> ()
            | PlayerCell playerCell ->
                match playerCell.Status with
                | Dead -> Draw.deadCell col row
                | Alive -> Draw.livingCell col row)

    let private renderControls controls =
        controls.Buttons |> Seq.iter (fun button -> Draw.button button)

    let render board controls =
        Raylib.BeginDrawing()
        Raylib.ClearBackground Color.White

        renderBoard board
        renderControls controls

        Raylib.EndDrawing()

    let close () = Raylib.CloseWindow()
