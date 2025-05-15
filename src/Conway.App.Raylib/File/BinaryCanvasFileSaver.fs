namespace Conway.App.Raylib.File

open Conway.App.Raylib
open Conway.Core

type BinaryCanvasFileSaver() =

    member this.Encode(grid: ConwayGrid) = failwith "Not Implemented"

    member this.Encode(game: Game) = failwith "Not Implemented"

    interface ICanvasFileSaver with
        member this.Save (canvas: Canvas) (path: string) : Result<Option<string>, string> = failwith "Not Implemented"
