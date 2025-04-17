namespace Conway.Core

open System.Threading

type GameMode =
    | Infinite
    | Limited of int

type Game(initialState: ConwayGrid) =
    let mutable internalState = initialState

    member val private mutex = new Mutex()

    member this.State
        with get () =
            try
                this.mutex.WaitOne() |> ignore
                internalState
            finally
                this.mutex.ReleaseMutex()
        and set newState =
            try
                this.mutex.WaitOne() |> ignore
                internalState <- newState
            finally
                this.mutex.ReleaseMutex()

    [<CompiledName("Run")>]
    member this.run mode =
        match mode with
        | Infinite ->
            while true do
                this.State <- ConwayGrid.next this.State
        | Limited steps ->
            for _ = 1 to steps do
                this.State <- ConwayGrid.next this.State

    member this.runOneStep() = this.run (Limited 1)
