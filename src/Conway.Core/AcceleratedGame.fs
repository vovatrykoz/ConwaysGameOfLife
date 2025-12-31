namespace Conway.Core

open System

[<Sealed>]
type AcceleratedGame(initialState: AcceleratedConwayGrid, generation: int) =
    let mutable _initialState = AcceleratedConwayGrid.copyFrom initialState

    let mutable _internalState = initialState

    let mutable _initialGeneration = generation

    let mutable _initialGeneration = generation

    let mutable _generation = generation

    new(initialState: AcceleratedConwayGrid) = new AcceleratedGame(initialState, 1)

    member _.CurrentState
        with get () = _internalState
        and set newState =
            _internalState <- newState
            _initialState <- AcceleratedConwayGrid.copyFrom newState
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
        _internalState <- AcceleratedConwayGrid.copyFrom _initialState
        _generation <- _initialGeneration

    member this.ResetGenerationCounter() =
        _initialState <- AcceleratedConwayGrid.copyFrom this.CurrentState
        _generation <- 1
        _initialGeneration <- _generation

    [<CompiledName("CreateFrom")>]
    static member createFrom
        (currentState: AcceleratedConwayGrid)
        (initialState: AcceleratedConwayGrid)
        (generationCounter: int)
        =
        let newGame = new AcceleratedGame(AcceleratedConwayGrid.copyFrom currentState)
        newGame.InitialState <- AcceleratedConwayGrid.copyFrom initialState
        newGame.Generation <- generationCounter
        newGame.StartingGeneration <- generationCounter
        newGame

    interface IDisposable with
        member this.Dispose() : unit =
            (this.CurrentState :> IDisposable).Dispose()
            (this.InitialState :> IDisposable).Dispose()
