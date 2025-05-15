namespace Conway.App.Raylib.File

open Conway.App.Raylib

type BinaryCanvasFileSaver() =
    interface ICanvasFileSaver with
        member this.Save (canvas: Canvas) (path: string) : Result<Option<string>, string> = failwith "Not Implemented"
