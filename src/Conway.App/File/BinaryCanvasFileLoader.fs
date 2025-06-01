namespace Conway.App.File

open Conway.App
open Conway.Encoding
open System
open System.IO

type BinaryCanvasFileLoader(decoder: IConwayByteDecoder) =

    member val Decoder = decoder with get, set

    member _.DecodeCameraInfo(cameraInfoBytes: ReadOnlySpan<byte>) =
        let posX = BitConverter.ToSingle(cameraInfoBytes.Slice(0, 4))
        let posY = BitConverter.ToSingle(cameraInfoBytes.Slice(4, 4))
        let zoomFactor = BitConverter.ToSingle(cameraInfoBytes.Slice(8, 4))

        Camera(posX, posY, zoomFactor)

    member this.Decode(byteEncoding: ReadOnlySpan<byte>) =
        let camera = this.DecodeCameraInfo(byteEncoding.Slice(0, 12))
        let game = decoder.Decode(byteEncoding.Slice(12).ToArray())

        game, camera

    interface ICanvasFileLoader with
        member this.Load(path: string) : Result<CanvasWrapper, string> =
            try
                let encoding = File.ReadAllBytes path
                let game, camera = this.Decode encoding

                Ok {
                    Game = game
                    Camera = camera
                    OptionalMessage = None
                }
            with _ ->
                Error "Something went wrong"
