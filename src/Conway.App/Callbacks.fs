namespace Conway.App

open Config
open Conway.App.Controls
open Conway.App.File
open Conway.App.Utils.Alias
open Conway.Core
open Conway.Encoding
open Raylib_cs
open System
open System.IO

module Callbacks =
    open Conway.App.Graphics
    open Conway.App.Input
    open System.Collections.Generic

    let saveFile (ctx: ApplicationContext) =
        try
            let saveFilesPath = Environment.CurrentDirectory + "/Saves"

            if not (Directory.Exists saveFilesPath) then
                Raylib.TraceLog(TraceLogLevel.Info, "Creating a save files directory...")
                Directory.CreateDirectory saveFilesPath |> ignore
                Raylib.TraceLog(TraceLogLevel.Info, "Save files directory created")

            let newFile = "./Saves/Test.gol"
            let encoder = new ConwayByteEncoder()
            let fileSaver = new BinaryCanvasFileSaver(encoder :> IConwayByteEncoder)

            Raylib.TraceLog(TraceLogLevel.Info, $"Saving the file to {newFile} ...")
            let result = (fileSaver :> ICanvasFileSaver).Save ctx.Canvas newFile

            match result with
            | Ok _ -> Raylib.TraceLog(TraceLogLevel.Info, "Test file saved successfully")
            | Error errorMessage ->
                Raylib.TraceLog(
                    TraceLogLevel.Error,
                    $"Could not save the file due to the following error: {errorMessage}"
                )
        with ex ->
            let excepionMessage = ex.Message.ToString().Replace("\n", "\n\t")

            Raylib.TraceLog(
                TraceLogLevel.Error,
                $"Failed to create a directory with the following error: {excepionMessage}"
            )

    let openFile (ctx: ApplicationContext) =
        try
            let saveFilesPath = Environment.CurrentDirectory + "/Saves"

            if not (Directory.Exists saveFilesPath) then
                Raylib.TraceLog(TraceLogLevel.Info, "Creating a save files directory...")
                Directory.CreateDirectory saveFilesPath |> ignore
                Raylib.TraceLog(TraceLogLevel.Info, "Save files directory created")

            let mutable isCancelled = false

            Raylib.SetExitKey KeyboardKey.Null

            let filePicker = new FilePicker(10.0f, 10.0f, 1000.0f, 480.0f, 1000.0f, 40.0f)
            filePicker.Files.CollectionChanged.Add(fun _ -> filePicker.ClearSelection())

            while not isCancelled
                  && not (raylibTrue (Raylib.WindowShouldClose()))
                  && not filePicker.Cancelled
                  && not filePicker.Confirmed do
                if
                    Keyboard.keyHasBeenPressedOnce KeyboardKey.Escape
                    || Keyboard.keyHasBeenPressedOnce KeyboardKey.Backspace
                then
                    isCancelled <- true
                else
                    let files =
                        Directory.GetFiles saveFilesPath
                        |> Array.map (fun fullPath ->
                            let fileName = Path.GetFileName fullPath
                            let lastModified = File.GetLastWriteTime fullPath
                            FileData.createRecord fileName fullPath lastModified)

                    files
                    |> Array.iter (fun fileData ->
                        if not (filePicker.Files.Contains fileData) then
                            filePicker.Files.Add fileData)

                    let removalIndeces = new List<int>()

                    filePicker.Files
                    |> Seq.iteri (fun index fileData ->
                        if not (Array.contains fileData files) then
                            removalIndeces.Add index)

                    removalIndeces.ForEach(fun index -> filePicker.Files.RemoveAt index)

                    Display.openFileDialogue ctx.Texture filePicker
                    filePicker.ProcessInput()

            Raylib.SetExitKey KeyboardKey.Escape

            if isCancelled || filePicker.Cancelled then
                ()
            else
                match filePicker.CurrentSelection with
                | None -> ()
                | Some fileData ->
                    let decoder = new ConwayByteDecoder()
                    let fileLoader = new BinaryCanvasFileLoader(decoder :> IConwayByteDecoder)

                    Raylib.TraceLog(TraceLogLevel.Info, $"Loading the file from {fileData.Path} ...")
                    let result = (fileLoader :> ICanvasFileLoader).Load fileData.Path

                    match result with
                    | Ok canvasWrapper ->
                        Raylib.TraceLog(TraceLogLevel.Info, "Test file loaded successfully")

                        match canvasWrapper.OptionalMessage with
                        | None -> ()
                        | Some message -> Raylib.TraceLog(TraceLogLevel.Info, message)

                        Raylib.TraceLog(TraceLogLevel.Info, "Updating the grid...")

                        ctx.Canvas.Game <-
                            Game.createFrom
                                canvasWrapper.Game.CurrentState
                                canvasWrapper.Game.InitialState
                                canvasWrapper.Game.Generation

                        ctx.Canvas.Camera <- canvasWrapper.Camera
                        Raylib.TraceLog(TraceLogLevel.Info, "Grid updated")
                    | Error errorMessage ->
                        Raylib.TraceLog(
                            TraceLogLevel.Error,
                            $"Could not load the file due to the following error: {errorMessage}"
                        )
        with ex ->
            let excepionMessage = ex.Message.ToString().Replace("\n", "\n\t")
            Raylib.TraceLog(TraceLogLevel.Error, $"Failed to load the save with the following error: {excepionMessage}")

    let toggleGame (ctx: ApplicationContext) =
        let currentGameMode = ctx.GameMode

        ctx.GameMode <-
            match currentGameMode with
            | GameRunMode.Paused -> GameRunMode.Infinite
            | _ -> GameRunMode.Paused

    let advanceOnce (ctx: ApplicationContext) =
        match ctx.GameMode with
        | GameRunMode.Paused -> ctx.Canvas.Game.RunOneStep()
        | _ -> ()

    let update (ctx: ApplicationContext) (button: Button) =
        match ctx.GameMode with
        | GameRunMode.Paused -> button.Text <- "Run"
        | _ -> button.Text <- "Pause"

    let updateOnRun (ctx: ApplicationContext) (button: Button) =
        match ctx.GameMode with
        | GameRunMode.Paused -> button.IsActive <- true
        | _ -> button.IsActive <- false

    let updateOnRunBack (ctx: ApplicationContext) (button: Button) =
        updateOnRun ctx button

        if button.IsActive && ctx.Canvas.Game.Generation <= 1 then
            button.IsActive <- false

    let resetCallback (ctx: ApplicationContext) =
        match ctx.GameMode with
        | GameRunMode.Paused -> ctx.Canvas.Game.ResetState()
        | _ -> ()

    let clearCallback (ctx: ApplicationContext) =
        let grid = ctx.Canvas.Game.CurrentState

        match ctx.GameMode with
        | GameRunMode.Paused -> ctx.Canvas.Game.CurrentState <- ConwayGrid.createDead grid.ActiveWidth grid.ActiveHeight
        | _ -> ()

    let fullscreenUpdate () =
        if raylibTrue (Raylib.IsKeyPressed KeyboardKey.Y) then
            if raylibTrue (Raylib.IsWindowFullscreen()) then
                Raylib.SetWindowSize(Default.windowWidth, Default.windowHeight)
            else
                let monitor = Raylib.GetCurrentMonitor()
                let monitorWidth = Raylib.GetMonitorWidth monitor
                let monitorHeight = Raylib.GetMonitorHeight monitor
                Raylib.SetWindowSize(monitorWidth, monitorHeight)

            Raylib.ToggleFullscreen()
