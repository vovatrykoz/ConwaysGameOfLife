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
    //
    [<Property(MaxTest = 1000)>]
    let ``Basic byte decoder and encoder can correctly encode and then restore a game while preserving the current state``
        (originalGame: Game)
        =
        let encoder = ConwayByteEncoder() :> IConwayByteEncoder
        let decoder = ConwayByteDecoder() :> IConwayByteDecoder

        let encodedGame = encoder.Encode originalGame
        let decodedGame = decoder.Decode encodedGame

        Assert.That(decodedGame.CurrentState.Board, Is.EqualTo originalGame.CurrentState.Board)

    [<Property(MaxTest = 1000)>]
    let ``Basic byte decoder and encoder can correctly encode and then restore a game while setting the inital state to the updated values``
        (originalGame: Game)
        =
        let encoder = ConwayByteEncoder() :> IConwayByteEncoder
        let decoder = ConwayByteDecoder() :> IConwayByteDecoder

        let encodedGame = encoder.Encode originalGame
        let decodedGame = decoder.Decode encodedGame

        Assert.That(decodedGame.InitialState.Board, Is.EqualTo originalGame.CurrentState.Board)

    [<Property(MaxTest = 1000)>]
    let ``Basic byte decoder and encoder can correctly encode and then restore a game while preserving the generation counter``
        (originalGame: Game)
        =
        let encoder = ConwayByteEncoder() :> IConwayByteEncoder
        let decoder = ConwayByteDecoder() :> IConwayByteDecoder

        let encodedGame = encoder.Encode originalGame
        let decodedGame = decoder.Decode encodedGame

        Assert.That(decodedGame.Generation, Is.EqualTo originalGame.Generation)

    [<Property(MaxTest = 1000)>]
    let ``Basic byte decoder and encoder can correctly encode and then restore a game while setting the starting generation counter to the updated value``
        (originalGame: Game)
        =
        let encoder = ConwayByteEncoder() :> IConwayByteEncoder
        let decoder = ConwayByteDecoder() :> IConwayByteDecoder

        let encodedGame = encoder.Encode originalGame
        let decodedGame = decoder.Decode encodedGame

        Assert.That(decodedGame.StartingGeneration, Is.EqualTo originalGame.Generation)

    [<Property(MaxTest = 1000)>]
    let ``Basic file saver can correctly write a file with game data and then load it back`` (originalCanvas: Canvas) =
        let encoder = ConwayByteEncoder() :> IConwayByteEncoder
        let decoder = ConwayByteDecoder() :> IConwayByteDecoder

        let fileSaver = BinaryCanvasFileSaver encoder :> ICanvasFileSaver
        let fileLoader = BinaryCanvasFileLoader decoder :> ICanvasFileLoader

        let location = "./TestSave"

        let _ = fileSaver.Save originalCanvas location
        let savedCanvas = fileLoader.Load location

        match savedCanvas with
        | Ok wrapper -> Assert.That(wrapper.Game.CurrentState.Board, Is.EqualTo originalCanvas.Game.CurrentState.Board)
        | Error err -> Assert.Fail $"Expected Ok, got {err}"
