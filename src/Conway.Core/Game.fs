namespace Conway.Core

[<Struct>]
type GameMode =
    | Infinite
    | Limited of int
    | Paused

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

    member this.Run(mode: GameMode) =
        match mode with
        | Infinite ->
            while true do
                this.CurrentState.AdvanceToNextState()
                this.Generation <- this.Generation + 1
        | Limited steps ->
            for _ = 1 to steps do
                this.CurrentState.AdvanceToNextState()
                this.Generation <- this.Generation + 1
        | Paused -> ()

    member this.RunOneStep() = this.Run(Limited 1)

    member _.ResetState() =
        _internalState <- ConwayGrid.copyFrom _initialState
        _generation <- 1

    member this.ResetGenerationCounter() =
        _initialState <- ConwayGrid.copyFrom this.CurrentState
        _generation <- 1
