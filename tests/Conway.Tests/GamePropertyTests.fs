namespace Conway.Tests

open Conway.Core
open NUnit.Framework
open FsCheck
open FsCheck.NUnit

[<Properties(Arbitrary = [| typeof<ConwayGen> |])>]
module ``Game Properties`` =
    //
    [<Property>]
    let ``Creating a new game through a primary constructor results in correctly set states and generations``
        (grid: ConwayGrid)
        (generation: int)
        =
        let game = new Game(grid, generation)

        Assert.Multiple(fun _ ->
            Assert.That(game.CurrentState, Is.EqualTo grid)
            Assert.That(game.InitialState, Is.Not.EqualTo grid)
            Assert.That(game.InitialState.Board, Is.EqualTo grid.Board)
            Assert.That(game.Generation, Is.EqualTo generation)
            Assert.That(game.StartingGeneration, Is.EqualTo generation))

    [<Property>]
    let ``Creating a new game through a constructor that only takes in a grid results in correctly set states and generations``
        (grid: ConwayGrid)
        =
        let game = new Game(grid)

        Assert.Multiple(fun _ ->
            Assert.That(game.CurrentState, Is.EqualTo grid)
            Assert.That(game.InitialState, Is.Not.EqualTo grid)
            Assert.That(game.InitialState.Board, Is.EqualTo grid.Board)
            Assert.That(game.Generation, Is.EqualTo 1)
            Assert.That(game.StartingGeneration, Is.EqualTo 1))

    [<Property>]
    let ``Game.CurrentState setter correctly sets all the values`` (oldState: ConwayGrid) (newState: ConwayGrid) =
        let game = new Game(oldState)
        game.CurrentState <- newState

        Assert.Multiple(fun _ ->
            Assert.That(game.CurrentState, Is.Not.EqualTo oldState)
            Assert.That(game.CurrentState, Is.EqualTo newState)
            Assert.That(game.InitialState, Is.Not.EqualTo newState)
            Assert.That(game.InitialState.Board, Is.EqualTo newState.Board)
            Assert.That(game.Generation, Is.EqualTo 1)
            Assert.That(game.StartingGeneration, Is.EqualTo 1))

    [<Property>]
    let ``Running a game one step forward produces correct new grid`` (grid: ConwayGrid) (generation: int) =
        let game = new Game(grid, generation)
        let copy = ConwayGrid.copyFrom grid
        copy.AdvanceToNextState()
        let expected = copy

        game.RunOneStep()

        Assert.That(game.CurrentState.Board, Is.EqualTo expected.Board)

    [<Property>]
    let ``Running a game one step forward correctly increments the generation counter``
        (grid: ConwayGrid)
        (generation: int)
        =
        let game = new Game(grid, generation)
        let expected = generation + 1
        game.RunOneStep()

        Assert.That(game.Generation, Is.EqualTo expected)

    [<Property>]
    let ``Resetting a game correctly resets the board`` (grid: ConwayGrid) (generation: int) (iterations: PositiveInt) =
        let game = new Game(grid, generation)

        for _ in 1 .. iterations.Get do
            game.RunOneStep()

        game.ResetState()

        Assert.Multiple(fun _ ->
            Assert.That(game.CurrentState, Is.Not.EqualTo game.InitialState)
            Assert.That(game.CurrentState.Board, Is.EqualTo game.InitialState.Board))

    [<Property>]
    let ``Resetting a game correctly resets the generation counter``
        (grid: ConwayGrid)
        (generation: int)
        (iterations: PositiveInt)
        =
        let game = new Game(grid, generation)

        for _ in 1 .. iterations.Get do
            game.RunOneStep()

        game.ResetState()

        Assert.That(game.Generation, Is.EqualTo generation)

    [<Property>]
    let ``Resetting a generation counter correctly resets the initial state``
        (grid: ConwayGrid)
        (generation: int)
        (iterations: PositiveInt)
        =
        let game = new Game(grid, generation)

        for _ in 1 .. iterations.Get do
            game.RunOneStep()

        game.ResetGenerationCounter()

        Assert.Multiple(fun _ ->
            Assert.That(game.InitialState, Is.Not.EqualTo game.CurrentState)
            Assert.That(game.InitialState.Board, Is.EqualTo game.CurrentState.Board))

    [<Property>]
    let ``Resetting a generation counter correctly resets it``
        (grid: ConwayGrid)
        (generation: int)
        (iterations: PositiveInt)
        =
        let game = new Game(grid, generation)

        for _ in 1 .. iterations.Get do
            game.RunOneStep()

        game.ResetGenerationCounter()

        Assert.Multiple(fun _ ->
            Assert.That(game.Generation, Is.EqualTo 1)
            Assert.That(game.StartingGeneration, Is.EqualTo 1))

    [<Property>]
    let ``Game.createFrom correctly initializes the new game``
        (currentGrid: ConwayGrid)
        (originalGrid: ConwayGrid)
        (generationCounter: int)
        =
        let game = Game.createFrom currentGrid originalGrid generationCounter

        Assert.Multiple(fun _ ->
            Assert.That(game.CurrentState, Is.Not.EqualTo currentGrid)
            Assert.That(game.CurrentState.Board, Is.EqualTo currentGrid.Board)
            Assert.That(game.InitialState, Is.Not.EqualTo originalGrid)
            Assert.That(game.InitialState.Board, Is.EqualTo originalGrid.Board)
            Assert.That(game.Generation, Is.EqualTo generationCounter)
            Assert.That(game.StartingGeneration, Is.EqualTo generationCounter))
