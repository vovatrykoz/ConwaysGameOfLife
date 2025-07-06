namespace Conway.App.Controls

open System
open System.Collections.Generic
open System.Numerics

[<ReferenceEquality; NoComparison>]
type FileData = {
    Name: string
    Path: string
    Date: DateTime
} with

    static member createRecord name path date = {
        Name = name
        Path = path
        Date = date
    }

type FilePicker(x: float32, y: float32) =
    member val Position = Vector2(x, y) with get, set

    member val Files = new List<FileData>() with get, set

    member val CurrentSelection: FileData option = None with get, set
