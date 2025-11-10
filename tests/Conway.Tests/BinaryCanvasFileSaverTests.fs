namespace Conway.Tests

open Conway.App
open Conway.App.Controls
open Conway.App.File
open Conway.App.Math
open Conway.Encoding
open Conway.Core
open NUnit.Framework
open System
open System.Collections

type FaultyEncoder<'T when 'T :> Exception>(exceptionToRaise: 'T) =

    member val ExceptionToRaise = exceptionToRaise with get

    static member willRaise<'T>(exceptionToRaise: 'T) = new FaultyEncoder<'T>(exceptionToRaise)

    interface ICanvasFileSaver with
        member this.Save (_: Canvas) (_: string) : unit = raise this.ExceptionToRaise

module ``Binary Canvas File Saver Tests`` =

    [<Test>]
    let ``Can correctly encode a simple canvas with default camera parameters`` () =
        let game = new Game(ConwayGrid.createDead 4 4)
        let camera = new Camera<cells>(0.0f<cells>, 0.0f<cells>)

        let canvas =
            new Canvas(0.0f<px>, 0.0f<px>, 100.0f<px>, 100.0f<px>, camera, game, 25.0f<px>)

        let expectedEncoding = [|
            0b0000_0000uy // camera X-coordinate (32-bit float)
            0b0000_0000uy // ...
            0b0000_0000uy // ...
            0b0000_0000uy // ...
            0b0000_0000uy // camera Y-coordinate (32-bit float)
            0b0000_0000uy // ...
            0b0000_0000uy // ...
            0b0000_0000uy // ...
            0b0000_0000uy // camera zoom factor (32-bit float)
            0b0000_0000uy // ...
            0b1000_0000uy // ...
            0b0011_1111uy // ...
            0b0000_0100uy // 4 rows (32-bit int)
            0b0000_0000uy // ...
            0b0000_0000uy // ...
            0b0000_0000uy // ...
            0b0000_0100uy // 4 cols (32-bit int)
            0b0000_0000uy // ...
            0b0000_0000uy // ...
            0b0000_0000uy // ...
            0b0000_0001uy // generation counter (32-bit int)
            0b0000_0000uy // ...
            0b0000_0000uy // ...
            0b0000_0000uy // ...
            0b0000_0000uy // 4x4 actual grid with all cells dead
            0b0000_0000uy // ...
            0b0000_0000uy // 4x4 initial grid with all cells dead
            0b0000_0000uy // ...
        |]

        let encoder = ConwayByteEncoder() :> IConwayByteEncoder
        let fileSaver = BinaryCanvasFileSaver encoder
        let actualEncoding = fileSaver.EncodeCanvasData canvas

        Assert.That(actualEncoding, Is.EqualTo<IEnumerable> expectedEncoding)
