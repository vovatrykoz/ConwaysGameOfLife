namespace Conway.Core

[<Struct>]
type GameMode =
    | Infinite
    | Limited of int
    | Paused

type Game(initialState: ConwayGrid) =
    let mutable _internalState = initialState

    let mutable _generation = 1

    member _.State
        with get () = _internalState
        and set newState =
            _internalState <- newState
            _generation <- 1

    member _.Generation
        with get () = _generation
        and private set newValue = _generation <- newValue

    member this.Run(mode: GameMode) =
        match mode with
        | Infinite ->
            while true do
                this.State.AdvanceToNextState()
                this.Generation <- this.Generation + 1
        | Limited steps ->
            for _ = 1 to steps do
                this.State.AdvanceToNextState()
                this.Generation <- this.Generation + 1
        | Paused -> ()

    member this.RunOneStep() = this.Run(Limited 1)

    member this.ClearHistory() = this.Generation <- 1
