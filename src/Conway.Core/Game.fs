namespace Conway.Core

type GameState =
    | Paused = 0
    | Step = 1
    | Infinite = 2

[<Sealed>]
type Game(initialState: ConwayGrid, generation: int) =
    let mutable _initialState = Array2D.copy initialState.Board

    let mutable _internalState = initialState

    let mutable _initialGeneration = generation

    let mutable _initialGeneration = generation

    let mutable _generation = generation

    new(initialState: ConwayGrid) = new Game(initialState, 1)

    member _.CurrentState
        with get () = _internalState
        and set newState =
            _internalState <- newState
            _initialState <- Array2D.copy newState.Board
            _generation <- 1
            _initialGeneration <- generation

    member _.StartingGrid
        with get () = _initialState
        and private set newValue = _initialState <- newValue

    member _.Generation
        with get () = _generation
        and private set newValue = _generation <- newValue

    member _.StartingGeneration
        with get () = _initialGeneration
        and private set newValue = _initialGeneration <- newValue

    member this.RunOneStep() =
        this.CurrentState.AdvanceToNextState()
        this.Generation <- this.Generation + 1

    member _.ResetState() =
        _internalState <- new ConwayGrid(_initialState)
        _generation <- _initialGeneration

    member this.ResetGenerationCounter() =
        _initialState <- Array2D.copy this.CurrentState.Board
        _generation <- 1
        _initialGeneration <- _generation

    [<CompiledName("CreateFrom")>]
    static member createFrom (currentState: ConwayGrid) (initialState: ConwayGrid) (generationCounter: int) =
        let newGame = new Game(ConwayGrid.copyFrom currentState)
        newGame.StartingGrid <- Array2D.copy initialState.Board
        newGame.Generation <- generationCounter
        newGame.StartingGeneration <- generationCounter
        newGame
