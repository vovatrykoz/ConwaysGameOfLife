namespace Conway.App.Controls

open Conway.App.Input
open Conway.Core
open Raylib_cs

module internal GameArea =
    let makeAlive row col (game: Game) =
        game.CurrentState.Board[row, col] <- 1<CellStatus>

        // erase the history since the player has altered the board
        game.ResetGenerationCounter()

    let makeDead row col (game: Game) =
        game.CurrentState.Board[row, col] <- 0<CellStatus>

        // erase the history since the player has altered the board
        game.ResetGenerationCounter()

    let IsPressedWithShift
        (startX: float32)
        (startY: float32)
        (endX: float32)
        (endY: float32)
        (mouseButton: MouseButton)
        =
        if
            (Keyboard.keyIsDown KeyboardKey.LeftShift
             || Keyboard.keyIsDown KeyboardKey.RightShift)
            && Mouse.buttonIsPressed mouseButton
        then
            let mousePos = Mouse.getPosition ()

            if
                mousePos.X >= startX
                && mousePos.X <= endX
                && mousePos.Y >= startY
                && mousePos.Y <= endY
            then
                true
            else
                false
        else
            false

    let IsLeftPressedWithShift (startX: float32) (startY: float32) (endX: float32) (endY: float32) =
        IsPressedWithShift startX startY endX endY MouseButton.Left

    let IsRightPressedWithShift (startX: float32) (startY: float32) (endX: float32) (endY: float32) =
        IsPressedWithShift startX startY endX endY MouseButton.Right
