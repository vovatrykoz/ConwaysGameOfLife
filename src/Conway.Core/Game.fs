namespace Conway.Core

type GameMode =
    | Infinite
    | Limited of int
    | Paused

type Game(initialState: ConwayGrid) =
    let mutable internalState = initialState

    let mutable generation = 1

    member this.State
        with get () = internalState
        and set newState = internalState <- newState

    member _.Generation
        with get () = generation
        and private set newValue = generation <- newValue

    [<CompiledName("Run")>]
    member this.run mode =
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

    [<CompiledName("RunOneStep")>]
    member this.runOneStep() = this.run (Limited 1)

    [<CompiledName("ClearHistory")>]
    member this.clearHistory() = this.Generation <- 1
