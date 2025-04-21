open Conway.Core
open Conway.App.Raylib
open Conway.App.Raylib.Aliases
open Raylib_cs

let width = 2001
let height = 2001

let startingState = ConwayGrid.createDead width height

let game = new Game(startingState)

let mainSync = obj ()

let withLock f = lock mainSync f

let mutable gameRunningState = Paused

let rec gameUpdateLoop state =
    async {
        do! Async.Sleep 100

        withLock (fun _ ->
            match gameRunningState with
            | Infinite -> game.runOneStep ()
            | Limited x when x > 1 ->
                game.runOneStep ()
                gameRunningState <- Limited(x - 1)
            | Limited _ ->
                game.runOneStep ()
                gameRunningState <- Paused
            | Paused -> ())

        return! gameUpdateLoop state
    }

let toggleGame () =
    withLock (fun _ ->
        gameRunningState <-
            match gameRunningState with
            | Paused -> Infinite
            | _ -> Paused)

let advanceOnce () =
    withLock (fun _ ->
        match gameRunningState with
        | Infinite
        | Limited _ -> ()
        | Paused -> game.runOneStep ())

let advanceBackOnce () =
    withLock (fun _ ->
        match gameRunningState with
        | Infinite
        | Limited _ -> ()
        | Paused -> game.stepBack ())

let update (button: Button) =
    withLock (fun _ ->
        match gameRunningState with
        | Paused -> button.Text <- "Run"
        | _ -> button.Text <- "Pause")

let updateOnRun (button: Button) =
    withLock (fun _ ->
        match gameRunningState with
        | Paused -> button.IsActive <- true
        | _ -> button.IsActive <- false)

let updateOnRunBack (button: Button) =
    updateOnRun button

    if button.IsActive && game.Generation <= 1 then
        button.IsActive <- false

let resetCallback () =
    withLock (fun _ ->
        match gameRunningState with
        | Infinite
        | Limited _ -> ()
        | Paused -> game.State <- ConwayGrid.createDead width height

        game.clearHistory ())

let clearCallback () =
    withLock (fun _ ->
        match gameRunningState with
        | Infinite
        | Limited _ -> ()
        | Paused -> game.State <- ConwayGrid.createDead width height

        game.clearHistory ())

let toggleButton =
    Button.create
    |> Button.position 600 300
    |> Button.size 50
    |> Button.onClickCallback toggleGame
    |> Button.onUpdateCallback update
    |> Button.shortcut KeyboardKey.Space

let advanceButton =
    Button.create
    |> Button.position 700 400
    |> Button.size 50
    |> Button.text "Next"
    |> Button.onClickCallback advanceOnce
    |> Button.onUpdateCallback updateOnRun
    |> Button.shortcut KeyboardKey.Right

let advanceBackButton =
    Button.create
    |> Button.position 600 400
    |> Button.size 50
    |> Button.text "Previous"
    |> Button.onClickCallback advanceBackOnce
    |> Button.onUpdateCallback updateOnRunBack
    |> Button.shortcut KeyboardKey.Left

let resetButton =
    Button.create
    |> Button.position 700 500
    |> Button.size 50
    |> Button.text "Reset"
    |> Button.onClickCallback resetCallback
    |> Button.onUpdateCallback updateOnRun

let clearButton =
    Button.create
    |> Button.position 600 500
    |> Button.size 50
    |> Button.text "Clear"
    |> Button.onClickCallback clearCallback
    |> Button.onUpdateCallback updateOnRun

let buttons = [ toggleButton; advanceButton; resetButton; clearButton ]

let controlManager = new ControlManager(game)
controlManager.AddButtons buttons

let keyboardActions = [
    KeyboardKey.W, controlManager.Canvas.MoveCameraUp
    KeyboardKey.A, controlManager.Canvas.MoveCameraLeft
    KeyboardKey.S, controlManager.Canvas.MoveCameraDown
    KeyboardKey.D, controlManager.Canvas.MoveCameraRight
]

controlManager.KeyActions <- keyboardActions

gameRunningState |> gameUpdateLoop |> Async.Start

Display.init ()

let renderTexture = Raylib.LoadRenderTexture(width, height)

while not (raylibTrue (Raylib.WindowShouldClose())) do
    controlManager.ReadInput()
    controlManager.UpdateControls()
    Display.render game controlManager renderTexture

Raylib.UnloadRenderTexture(renderTexture)
Display.close ()
