module Conway.Integration

open Conway.App
open Conway.App.File
open Conway.Core
open Conway.Encoding
open Conway.Tests
open NUnit.Framework
open FsCheck
open FsCheck.NUnit

[<Properties(Arbitrary = [| typeof<ConwayGen> |])>]
module ``Encoding Integration Tests`` =
    open System.IO

    // FsCheck test count increased from 100 to 1000
    // With 100, rare corner cases were sometimes missed, allowing buggy code to pass
    // With 1000, those cases are consistently covered
    [<Property(MaxTest = 1000)>]
    let ``Basic byte encoder and decoder can correctly encode and then restore a game while preserving the current state``
        (originalGame: Game)
        =
        let encoder = ConwayByteEncoder() :> IConwayByteEncoder
        let decoder = ConwayByteDecoder() :> IConwayByteDecoder

        let encodedGame = encoder.Encode originalGame
        let decodedGame = decoder.Decode encodedGame

        Assert.That(decodedGame.CurrentState.Board, Is.EqualTo originalGame.CurrentState.Board)

    [<Property(MaxTest = 1000)>]
    let ``Basic byte encoder and decoder can correctly encode and then restore a game while setting the initial state to the updated values``
        (originalGame: Game)
        =
        let encoder = ConwayByteEncoder() :> IConwayByteEncoder
        let decoder = ConwayByteDecoder() :> IConwayByteDecoder

        let encodedGame = encoder.Encode originalGame
        let decodedGame = decoder.Decode encodedGame

        Assert.That(decodedGame.InitialState.Board, Is.EqualTo originalGame.CurrentState.Board)

    [<Property(MaxTest = 1000)>]
    let ``Basic byte encoder and decoder can correctly encode and then restore a game while preserving the generation counter``
        (originalGame: Game)
        =
        let encoder = ConwayByteEncoder() :> IConwayByteEncoder
        let decoder = ConwayByteDecoder() :> IConwayByteDecoder

        let encodedGame = encoder.Encode originalGame
        let decodedGame = decoder.Decode encodedGame

        Assert.That(decodedGame.Generation, Is.EqualTo originalGame.Generation)

    [<Property(MaxTest = 1000)>]
    let ``Basic byte encoder and decoder can correctly encode and then restore a game while setting the starting generation counter to the current value``
        (originalGame: Game)
        =
        let encoder = ConwayByteEncoder() :> IConwayByteEncoder
        let decoder = ConwayByteDecoder() :> IConwayByteDecoder

        let encodedGame = encoder.Encode originalGame
        let decodedGame = decoder.Decode encodedGame

        Assert.That(decodedGame.StartingGeneration, Is.EqualTo originalGame.Generation)

    [<Property(MaxTest = 1000)>]
    let ``Basic file saver can correctly write a file with game data and then load it back, preserving the board state``
        (originalCanvas: Canvas)
        =
        let encoder = ConwayByteEncoder() :> IConwayByteEncoder
        let decoder = ConwayByteDecoder() :> IConwayByteDecoder

        let fileSaver = BinaryCanvasFileSaver encoder :> ICanvasFileSaver
        let fileLoader = BinaryCanvasFileLoader decoder :> ICanvasFileLoader

        let location = Path.GetTempFileName()

        let _ = fileSaver.Save originalCanvas location
        let savedCanvas = fileLoader.Load location

        File.Delete location

        Assert.That(savedCanvas.Game.CurrentState.Board, Is.EqualTo originalCanvas.Game.CurrentState.Board)

    [<Property(MaxTest = 1000)>]
    let ``Basic file saver can correctly write a file with game data and then load it back, setting the loaded state as the initial board state``
        (originalCanvas: Canvas)
        =
        let encoder = ConwayByteEncoder() :> IConwayByteEncoder
        let decoder = ConwayByteDecoder() :> IConwayByteDecoder

        let fileSaver = BinaryCanvasFileSaver encoder :> ICanvasFileSaver
        let fileLoader = BinaryCanvasFileLoader decoder :> ICanvasFileLoader

        let location = Path.GetTempFileName()

        let _ = fileSaver.Save originalCanvas location
        let savedCanvas = fileLoader.Load location

        File.Delete location

        Assert.That(savedCanvas.Game.InitialState.Board, Is.EqualTo originalCanvas.Game.CurrentState.Board)

    [<Property(MaxTest = 1000)>]
    let ``Basic file saver can correctly write a file with game data and then load it back while preserving counters``
        (originalCanvas: Canvas)
        =
        let encoder = ConwayByteEncoder() :> IConwayByteEncoder
        let decoder = ConwayByteDecoder() :> IConwayByteDecoder

        let fileSaver = BinaryCanvasFileSaver encoder :> ICanvasFileSaver
        let fileLoader = BinaryCanvasFileLoader decoder :> ICanvasFileLoader

        let location = Path.GetTempFileName()

        let _ = fileSaver.Save originalCanvas location
        let savedCanvas = fileLoader.Load location

        File.Delete location

        Assert.Multiple(fun _ ->
            Assert.That(savedCanvas.Game.Generation, Is.EqualTo originalCanvas.Game.Generation)
            Assert.That(savedCanvas.Game.StartingGeneration, Is.EqualTo originalCanvas.Game.Generation))

    [<Property(MaxTest = 1000)>]
    let ``Basic file saver can correctly write a file with game data and then load it back while preserving camera settings``
        (originalCanvas: Canvas)
        =
        let encoder = ConwayByteEncoder() :> IConwayByteEncoder
        let decoder = ConwayByteDecoder() :> IConwayByteDecoder

        let fileSaver = BinaryCanvasFileSaver encoder :> ICanvasFileSaver
        let fileLoader = BinaryCanvasFileLoader decoder :> ICanvasFileLoader

        let location = Path.GetTempFileName()

        let _ = fileSaver.Save originalCanvas location
        let savedCanvas = fileLoader.Load location

        Assert.Multiple(fun _ ->
            Assert.That(savedCanvas.Camera.Position.X, Is.EqualTo originalCanvas.Camera.Position.X)
            Assert.That(savedCanvas.Camera.Position.Y, Is.EqualTo originalCanvas.Camera.Position.Y)
            Assert.That(savedCanvas.Camera.ZoomFactor, Is.EqualTo originalCanvas.Camera.ZoomFactor))
