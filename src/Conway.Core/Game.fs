namespace Conway.Core

type GameRunMode =
    | Paused = 0
    | Step = 1
    | Infinite = 2

type Game(initialState: ConwayGrid, generation: int) =
    let mutable _initialState = ConwayGrid.copyFrom initialState

    let mutable _internalState = initialState

    let mutable _generation = generation

    new(initialState: ConwayGrid) = new Game(initialState, 1)

    member _.CurrentState
        with get () = _internalState
        and set newState =
            _internalState <- newState
            _initialState <- ConwayGrid.copyFrom newState
            _generation <- 1

    member _.InitialState
        with get () = _initialState
        and private set newValue = _initialState <- newValue

    member _.Generation
        with get () = _generation
        and private set newValue = _generation <- newValue

    member this.Run(mode: GameRunMode) =
        match mode with
        | GameRunMode.Infinite ->
            while true do
                this.CurrentState.AdvanceToNextState()
                this.Generation <- this.Generation + 1
        | GameRunMode.Step ->
            this.CurrentState.AdvanceToNextState()
            this.Generation <- this.Generation + 1
        | GameRunMode.Paused
        | _ -> ()

    member this.RunOneStep() = this.Run GameRunMode.Step

    member _.ResetState() =
        _internalState <- ConwayGrid.copyFrom _initialState
        _generation <- 1

    member this.ResetGenerationCounter() =
        _initialState <- ConwayGrid.copyFrom this.CurrentState
        _generation <- 1

    [<CompiledName("CreateFrom")>]
    static member createFrom (currentState: ConwayGrid) (initialState: ConwayGrid) (generationCounter: int) =
        let newGame = new Game(ConwayGrid.copyFrom currentState)
        newGame.InitialState <- ConwayGrid.copyFrom initialState
        newGame.Generation <- generationCounter
        newGame
