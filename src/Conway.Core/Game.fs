namespace Conway.Core

type GameState =
    | Paused = 0
    | Step = 1
    | Infinite = 2

type Game(initialState: ConwayGrid, generation: int) =
    let mutable _initialState = ConwayGrid.copyFrom initialState

    let mutable _internalState = initialState

    let mutable _initialGeneration = generation

    let mutable _initialGeneration = generation

    let mutable _generation = generation

    new(initialState: ConwayGrid) = new Game(initialState, 1)

    member _.CurrentState
        with get () = _internalState
        and set newState =
            _internalState <- newState
            _initialState <- ConwayGrid.copyFrom newState
            _generation <- 1
            _initialGeneration <- generation

    member _.InitialState
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
        _internalState <- ConwayGrid.copyFrom _initialState
        _generation <- _initialGeneration

    member this.ResetGenerationCounter() =
        _initialState <- ConwayGrid.copyFrom this.CurrentState
        _generation <- 1
        _initialGeneration <- _generation

    [<CompiledName("CreateFrom")>]
    static member createFrom (currentState: ConwayGrid) (initialState: ConwayGrid) (generationCounter: int) =
        let newGame = new Game(ConwayGrid.copyFrom currentState)
        newGame.InitialState <- ConwayGrid.copyFrom initialState
        newGame.Generation <- generationCounter
        newGame.StartingGeneration <- generationCounter
        newGame
