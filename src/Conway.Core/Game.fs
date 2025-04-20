namespace Conway.Core

open System.Threading

type GameMode =
    | Infinite
    | Limited of int
    | Paused

type Game(initialState: ConwayGrid) =
    let mutable internalState = initialState

    let mutable generation = 1

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

    member _.Generation
        with get () = generation
        and private set newValue = generation <- newValue

    [<CompiledName("Run")>]
    member this.run mode =
        match mode with
        | Infinite ->
            while true do
                ConwayGrid.next this.State |> ignore
                this.Generation <- this.Generation + 1
        | Limited steps ->
            for _ = 1 to steps do
                ConwayGrid.next this.State |> ignore
                this.Generation <- this.Generation + 1
        | Paused -> ()

    [<CompiledName("RunOneStep")>]
    member this.runOneStep() = this.run (Limited 1)

    [<CompiledName("StepBack")>]
    member this.stepBack() =
        ConwayGrid.previous this.State |> ignore

        if this.Generation > 1 then
            this.Generation <- this.Generation - 1

    [<CompiledName("ClearHistory")>]
    member this.clearHistory() =
        this.State.Board
        |> Array2D.iteri (fun row col cell ->
            match cell with
            | BorderCell -> this.State.Board[row, col] <- BorderCell
            | PlayerCell playerCell -> this.State.Board[row, col] <- PlayerCell { playerCell with Memory = Stack.empty })

        this.Generation <- 1

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
