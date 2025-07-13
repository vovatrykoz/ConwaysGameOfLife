namespace Conway.App.File

open Conway.App.Controls
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
            with
            | :? ArgumentNullException as argNullEx ->
                Error $"Could not open the file. {argNullEx.ParamName} was null or empty"
            | :? ArgumentException ->
                if String.IsNullOrEmpty path then
                    Error $"{nameof path} was empty"
                else if String.IsNullOrWhiteSpace path then
                    Error $"{nameof path} consisted of whitespaces only"
                else
                    let invalidCharSet = Path.GetInvalidPathChars() |> Set.ofArray

                    let invalidChars =
                        path |> Seq.filter (fun c -> Set.contains c invalidCharSet) |> Seq.toArray

                    Error $"{nameof path} contained the following invalid characters: {invalidChars}"
            | :? PathTooLongException -> Error $"The provided {nameof path} was too long: {path}"
            | :? DirectoryNotFoundException -> Error $"The provided {nameof path} was not found: {path}"
            | :? IOException -> Error $"An I/O error occurred while opening the file at {path}. Does the file exist?"
            | :? Security.SecurityException -> Error $"The caller does not have enough permissions to open the file"
            | :? UnauthorizedAccessException ->
                Error
                    $"Access to the file at {path} was denied. This could be due to one of the following:
                    (1) the caller does not have the required permission
                    (2) {path} is a directory
                    (3) operation is not supported on the current platform"
            | :? NotSupportedException -> Error $"The provided {nameof path} has an invalid format: {path}"
            | _ -> Error "Something went wrong. See the log output for details"
