namespace Conway.App.File

open Conway.App
open Conway.App.Math
open Conway.App.Controls
open Conway.Encoding
open Raylib_cs
open System
open System.IO

type BinaryCanvasFileSaver(gameEncoder: IConwayByteEncoder) =

    member val Encoder = gameEncoder with get

    member _.EncodeCameraData(camera: Camera<cells>) =
        let encodedPositionX = BitConverter.GetBytes(float32 camera.Position.X)
        let encodedPositionY = BitConverter.GetBytes(float32 camera.Position.Y)
        let encodedZoomFactor = BitConverter.GetBytes camera.ZoomFactor

        encodedZoomFactor
        |> Array.append encodedPositionY
        |> Array.append encodedPositionX

    member this.EncodeCanvasData(canvas: Canvas) =
        let gameDataEncoded = this.Encoder.Encode canvas.Game
        let cameraDataEncoded = this.EncodeCameraData canvas.Camera

        Array.append cameraDataEncoded gameDataEncoded

    interface ICanvasFileSaver with
        member this.Save (canvas: Canvas) (path: string) : unit =
            let completeEncoding = this.EncodeCanvasData canvas

            try
                File.WriteAllBytes(path, completeEncoding)
                Raylib.TraceLog(TraceLogLevel.Info, $"File saved at {path}")
            with
            | :? ArgumentNullException as argNullEx ->
                Raylib.TraceLog(
                    TraceLogLevel.Error,
                    $"Could not save the file. {argNullEx.ParamName} was null or empty"
                )

                reraise ()

            | :? ArgumentException ->
                if String.IsNullOrEmpty path then
                    Raylib.TraceLog(TraceLogLevel.Error, $"{nameof path} was empty")
                    reraise ()
                elif String.IsNullOrWhiteSpace path then
                    Raylib.TraceLog(TraceLogLevel.Error, $"{nameof path} consisted of whitespaces only")
                    reraise ()
                else
                    let invalidCharSet = Path.GetInvalidPathChars() |> Set.ofArray

                    let invalidChars =
                        path |> Seq.filter (fun c -> Set.contains c invalidCharSet) |> Seq.toArray

                    Raylib.TraceLog(
                        TraceLogLevel.Error,
                        $"{nameof path} contained the following invalid characters: {invalidChars}"
                    )

                    reraise ()

            | :? PathTooLongException ->
                Raylib.TraceLog(TraceLogLevel.Error, $"The provided {nameof path} was too long: {path}")
                reraise ()

            | :? DirectoryNotFoundException ->
                Raylib.TraceLog(TraceLogLevel.Error, $"The provided {nameof path} was not found: {path}")
                reraise ()

            | :? IOException ->
                Raylib.TraceLog(TraceLogLevel.Error, $"An I/O error occurred while saving the file at {path}.")
                reraise ()

            | :? Security.SecurityException ->
                Raylib.TraceLog(TraceLogLevel.Error, $"The caller does not have enough permissions to save the file")
                reraise ()

            | :? UnauthorizedAccessException ->
                Raylib.TraceLog(
                    TraceLogLevel.Error,
                    $"Access to the file at {path} was denied. This could be due to one of the following:
                    (1) the caller does not have the required permission
                    (2) {path} is a directory
                    (3) file is readonly
                    (4) file is hidden
                    (5) operation is not supported on the current platform"
                )

                reraise ()

            | :? NotSupportedException ->
                Raylib.TraceLog(TraceLogLevel.Error, $"The provided {nameof path} has an invalid format: {path}")
                reraise ()

            | _ ->
                Raylib.TraceLog(TraceLogLevel.Error, "Something went wrong. Please try again")
                reraise ()
