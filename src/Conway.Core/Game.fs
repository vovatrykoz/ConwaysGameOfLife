namespace Conway.Core

[<Measure>]
type GameMode

type Game(initialState: ConwayGrid) =
    let mutable _initialState = ConwayGrid.copyFrom initialState

    let mutable _internalState = initialState

    let mutable _generation = 1

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

    member this.Run(mode: int<GameMode>) =
        match mode with
        | 2<GameMode> ->
            while true do
                this.CurrentState.AdvanceToNextState()
                this.Generation <- this.Generation + 1
        | 1<GameMode> ->
            this.CurrentState.AdvanceToNextState()
            this.Generation <- this.Generation + 1
        | 0<GameMode>
        | _ -> ()

    member this.RunOneStep() = this.Run 1<GameMode>

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
