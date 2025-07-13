namespace Conway.App.File

open Conway.App.Controls
open Conway.Encoding
open System
open System.IO

type BinaryCanvasFileSaver(encoder: IConwayByteEncoder) =

    member val Encoder = encoder with get, set

    member _.EncodeCameraData(camera: Camera) =
        let encodedPositionX = BitConverter.GetBytes camera.Position.X
        let encodedPositionY = BitConverter.GetBytes camera.Position.Y
        let encodedZoomFactor = BitConverter.GetBytes camera.ZoomFactor

        encodedZoomFactor
        |> Array.append encodedPositionY
        |> Array.append encodedPositionX

    member this.EncodeCanvasData(canvas: Canvas) =
        let gameDataEncoded = this.Encoder.Encode canvas.Game
        let cameraDataEncoded = this.EncodeCameraData canvas.Camera

        Array.append cameraDataEncoded gameDataEncoded

    interface ICanvasFileSaver with
        member this.Save (canvas: Canvas) (path: string) : Result<Option<string>, string> =
            let completeEncoding = this.EncodeCanvasData canvas

            try
                File.WriteAllBytes(path, completeEncoding)
                Ok(Some $"File saved at {path}")
            with
            | :? ArgumentNullException as argNullEx ->
                Error $"Could not save the file. {argNullEx.ParamName} was null or empty"
            | :? ArgumentException ->
                if String.IsNullOrEmpty path then
                    Error $"{nameof path} was empty"
                else if String.IsNullOrWhiteSpace path then
                    Error $"{nameof path} consisted of whitespaces only"
                else
                    let invalidCharSet = Path.GetInvalidPathChars() |> Set.ofSeq

                    let invalidChars =
                        path |> Seq.filter (fun c -> Set.contains c invalidCharSet) |> Seq.toArray

                    Error $"{nameof path} contained the following invalid characters: {invalidChars}"
            | :? PathTooLongException -> Error $"The provided {nameof path} was too long: {path}"
            | :? DirectoryNotFoundException -> Error $"The provided {nameof path} was not found: {path}"
            | :? IOException -> Error $"An I/O error occurred while opening the file at {path}"
            | :? Security.SecurityException -> Error $"The caller does not have enough permissions to save the file"
            | :? UnauthorizedAccessException ->
                Error
                    $"Access to the file at {path} was denied. This could be due to one of the following:
                    (1) the caller does not have the required permission
                    (2) {path} is a directory
                    (3) file is readonly
                    (4) file is hidden
                    (5) operation is not supported on the current platform"
            | :? NotSupportedException -> Error $"The provided {nameof path} has an invalid format: {path}"
            | _ -> Error "Something went wrong. Please try again"
