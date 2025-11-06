namespace Conway.App

open Config
open Conway.App.Controls
open Conway.App.Math
open Raylib_cs

module Buttons =
    let createSaveButtonInstance (ctx: ApplicationContext) =
        Button.create
        |> Button.position (Default.windowWidth - 200<px>) (Default.windowHeight - 400<px>)
        |> Button.width 50<px>
        |> Button.height 50<px>
        |> Button.text "Save"
        |> Button.onClickCallback (fun _ -> Callbacks.saveFile ctx)
        |> Button.onUpdateCallback (fun btn -> Callbacks.updateOnRun ctx btn)

    let createLoadButtonInstance (ctx: ApplicationContext) =
        Button.create
        |> Button.position (Default.windowWidth - 100<px>) (Default.windowHeight - 400<px>)
        |> Button.width 50<px>
        |> Button.height 50<px>
        |> Button.text "Load"
        |> Button.onClickCallback (fun _ -> Callbacks.openFile ctx)
        |> Button.onUpdateCallback (fun btn -> Callbacks.updateOnRun ctx btn)

    let createRunButtonInstance (ctx: ApplicationContext) =
        Button.create
        |> Button.position (Default.windowWidth - 200<px>) (Default.windowHeight - 300<px>)
        |> Button.width 50<px>
        |> Button.height 50<px>
        |> Button.onClickCallback (fun _ -> Callbacks.toggleGame ctx)
        |> Button.onUpdateCallback (fun btn -> Callbacks.update ctx btn)
        |> Button.shortcut KeyboardKey.Space

    let createRandomizeButtonInstance (ctx: ApplicationContext) =
        Button.create
        |> Button.position (Default.windowWidth - 200<px>) (Default.windowHeight - 200<px>)
        |> Button.width 50<px>
        |> Button.height 50<px>
        |> Button.text "Rand"
        |> Button.onClickCallback (fun _ -> Callbacks.randomize ctx)
        |> Button.onUpdateCallback (fun btn -> Callbacks.updateOnRun ctx btn)
        |> Button.shortcut KeyboardKey.Space

    let createAdvanceButtonInstance (ctx: ApplicationContext) =
        Button.create
        |> Button.position (Default.windowWidth - 100<px>) (Default.windowHeight - 200<px>)
        |> Button.width 50<px>
        |> Button.height 50<px>
        |> Button.text "Next"
        |> Button.onClickCallback (fun _ -> Callbacks.advanceOnce ctx)
        |> Button.onUpdateCallback (fun btn -> Callbacks.updateOnRun ctx btn)
        |> Button.shortcut KeyboardKey.Right

    let createResetButtonInstance (ctx: ApplicationContext) =
        Button.create
        |> Button.position (Default.windowWidth - 100<px>) (Default.windowHeight - 100<px>)
        |> Button.width 50<px>
        |> Button.height 50<px>
        |> Button.text "Reset"
        |> Button.onClickCallback (fun _ -> Callbacks.resetCallback ctx)
        |> Button.onUpdateCallback (fun btn -> Callbacks.updateOnRun ctx btn)

    let createClearButtonInstance (ctx: ApplicationContext) =
        Button.create
        |> Button.position (Default.windowWidth - 200<px>) (Default.windowHeight - 100<px>)
        |> Button.width 50<px>
        |> Button.height 50<px>
        |> Button.text "Clear"
        |> Button.onClickCallback (fun _ -> Callbacks.clearCallback ctx)
        |> Button.onUpdateCallback (fun btn -> Callbacks.updateOnRun ctx btn)

    let instantiate (ctx: ApplicationContext) = [|
        createRunButtonInstance ctx
        createRandomizeButtonInstance ctx
        createAdvanceButtonInstance ctx
        createResetButtonInstance ctx
        createClearButtonInstance ctx
        createSaveButtonInstance ctx
        createLoadButtonInstance ctx
    |]
