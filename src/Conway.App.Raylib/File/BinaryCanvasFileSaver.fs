namespace Conway.App.Raylib.File

open Conway.App.Raylib
open Conway.Core
open Conway.Encoding

type BinaryCanvasFileSaver(encoder: IConwayByteEncoder) =

    member val Encoder = encoder with get, set

    interface ICanvasFileSaver with
        member this.Save (canvas: Canvas) (path: string) : Result<Option<string>, string> = failwith "Not Implemented"
