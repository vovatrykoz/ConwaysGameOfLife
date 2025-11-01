module Conway.Integration

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
