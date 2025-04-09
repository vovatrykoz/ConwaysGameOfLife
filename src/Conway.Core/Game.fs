namespace Conway.Core

type GameMode =
    | Infinite
    | Limited of int

type Game(initialState: Grid) =
    member val State = initialState with get, set

    [<CompiledName("Run")>]
    member this.run mode =
        match mode with
        | Infinite ->
            while true do
                this.State <- Grid.next this.State
        | Limited steps ->
            for _ = 1 to steps do
                this.State <- Grid.next this.State

    member this.runOneStep() = this.run (Limited 1)
