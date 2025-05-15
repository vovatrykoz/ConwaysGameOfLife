namespace Conway.App.Raylib.File

open Conway.App.Raylib
open Conway.Core
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

    interface ICanvasFileSaver with
        member this.Save (canvas: Canvas) (path: string) : Result<Option<string>, string> =
            let gameDataEncoded = this.Encoder.Encode canvas.Game
            let cameraDataEncoded = this.EncodeCameraData canvas.Camera
            let completeEncoding = Array.append gameDataEncoded cameraDataEncoded

            try
                File.WriteAllBytes(path, completeEncoding)
                Ok(Some $"File saved at {path}")
            with _ ->
                Error "Could not save the file"
