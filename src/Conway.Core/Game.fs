namespace Conway.Core

type GameMode =
    | Infinite
    | Limited of int
    | Paused

type Game(initialState: ConwayGrid) =
    let mutable internalState = initialState

    let mutable generation = 1

    member _.State
        with get () = internalState
        and set newState = internalState <- newState

    member _.Generation
        with get () = generation
        and private set newValue = generation <- newValue

    member this.Run(mode: GameMode) =
        match mode with
        | Infinite ->
            while true do
                this.State.AdvanceToNextState() |> ignore
                this.Generation <- this.Generation + 1
        | Limited steps ->
            for _ = 1 to steps do
                this.State.AdvanceToNextState() |> ignore
                this.Generation <- this.Generation + 1
        | Paused -> ()

    member this.RunOneStep() = this.Run(Limited 1)

    member this.ClearHistory() = this.Generation <- 1
