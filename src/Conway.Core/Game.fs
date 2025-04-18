namespace Conway.Core

open System.Threading

type GameMode =
    | Infinite
    | Limited of int
    | Paused

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
        | Paused -> ()

    [<CompiledName("RunOneStep")>]
    member this.runOneStep() = this.run (Limited 1)

    [<CompiledName("StepBack")>]
    member this.stepBack() =
        this.State <- ConwayGrid.previous this.State

    [<CompiledName("ClearHistory")>]
    member this.clearHistory() =
        let newBoard =
            this.State.Board
            |> Array2D.map (fun cell ->
                match cell with
                | BorderCell -> BorderCell
                | PlayerCell playerCell -> PlayerCell { playerCell with Memory = Stack.empty })

        this.State <- { this.State with Board = newBoard }

    [<CompiledName("HasMemoryLoss")>]
    member this.hasMemoryLoss() =
        let mutable memoryLossPresent = false

        for i in 0 .. (Array2D.length1 this.State.Board - 1) do
            for j in 0 .. (Array2D.length2 this.State.Board - 1) do
                match this.State.Board[i, j] with
                | PlayerCell playerCell ->
                    if Stack.isEmpty playerCell.Memory then
                        memoryLossPresent <- true
                    else
                        ()
                | BorderCell -> ()

        memoryLossPresent
