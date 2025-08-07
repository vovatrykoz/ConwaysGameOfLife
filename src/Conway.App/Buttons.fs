namespace Conway.App

open Config
open Conway.App.Controls
open Raylib_cs

module Buttons =
    let createSaveButtonInstance (ctx: ApplicationContext) =
        Button.create
        |> Button.position (Default.windowWidth - 200) (Default.windowHeight - 400)
        |> Button.width 50
        |> Button.height 50
        |> Button.text "Save"
        |> Button.onClickCallback (fun _ -> Callbacks.saveFile ctx)
        |> Button.onUpdateCallback (fun btn -> Callbacks.updateOnRun ctx btn)

    let createLoadButtonInstance (ctx: ApplicationContext) =
        Button.create
        |> Button.position (Default.windowWidth - 100) (Default.windowHeight - 400)
        |> Button.width 50
        |> Button.height 50
        |> Button.text "Load"
        |> Button.onClickCallback (fun _ -> Callbacks.openFile ctx)
        |> Button.onUpdateCallback (fun btn -> Callbacks.updateOnRun ctx btn)

    let createRunButtonInstance (ctx: ApplicationContext) =
        Button.create
        |> Button.position (Default.windowWidth - 200) (Default.windowHeight - 200)
        |> Button.width 50
        |> Button.height 50
        |> Button.onClickCallback (fun _ -> Callbacks.toggleGame ctx)
        |> Button.onUpdateCallback (fun btn -> Callbacks.update ctx btn)
        |> Button.shortcut KeyboardKey.Space

    let createAdvanceButtonInstance (ctx: ApplicationContext) =
        Button.create
        |> Button.position (Default.windowWidth - 100) (Default.windowHeight - 200)
        |> Button.width 50
        |> Button.height 50
        |> Button.text "Next"
        |> Button.onClickCallback (fun _ -> Callbacks.advanceOnce ctx)
        |> Button.onUpdateCallback (fun btn -> Callbacks.updateOnRun ctx btn)
        |> Button.shortcut KeyboardKey.Right

    let createResetButtonInstance (ctx: ApplicationContext) =
        Button.create
        |> Button.position (Default.windowWidth - 100) (Default.windowHeight - 100)
        |> Button.width 50
        |> Button.height 50
        |> Button.text "Reset"
        |> Button.onClickCallback (fun _ -> Callbacks.resetCallback ctx)
        |> Button.onUpdateCallback (fun btn -> Callbacks.updateOnRun ctx btn)

    let createClearButtonInstance (ctx: ApplicationContext) =
        Button.create
        |> Button.position (Default.windowWidth - 200) (Default.windowHeight - 100)
        |> Button.width 50
        |> Button.height 50
        |> Button.text "Clear"
        |> Button.onClickCallback (fun _ -> Callbacks.clearCallback ctx)
        |> Button.onUpdateCallback (fun btn -> Callbacks.updateOnRun ctx btn)

    let instantiate (ctx: ApplicationContext) = [|
        createRunButtonInstance ctx
        createAdvanceButtonInstance ctx
        createResetButtonInstance ctx
        createClearButtonInstance ctx
        createSaveButtonInstance ctx
        createLoadButtonInstance ctx
    |]
