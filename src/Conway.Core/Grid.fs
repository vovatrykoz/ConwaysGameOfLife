namespace Conway.Core

open System
open System.Runtime.CompilerServices
open System.Threading.Tasks
open Microsoft.FSharp.NativeInterop

module private FastHelpers =
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    let retype<'T> (value: 'T) = (# "" value: int32 #)

[<NoComparison>]
type ConwayGrid private (startingGrid: Cell array2d) =

    private new(width: int, height: int) =
        let initArr = Array2D.create (height + 2) (width + 2) Cell.dead

        new ConwayGrid(initArr)

    member val private Buffers = [| Array2D.copy startingGrid; Array2D.copy startingGrid |] with get, set

    member val private ActiveBufferIndex = 0 with get, set

    member this.Board = this.Buffers[this.ActiveBufferIndex]

    member this.AdvanceToNextState() =
        let activeIndex = this.ActiveBufferIndex
        let passiveIndex = (activeIndex + 1) % this.Buffers.Length
        let activeBuffer = this.Buffers[activeIndex]
        let passiveBuffer = this.Buffers[passiveIndex]

        let rows = Array2D.length1 activeBuffer
        let cols = Array2D.length2 activeBuffer

        Parallel.For(
            1,
            rows - 1,
            fun row ->
                for col in 1 .. cols - 2 do
                    passiveBuffer[row, col] <- ConwayGrid.evolveCellAt row col activeBuffer (activeBuffer[row, col])
        )
        |> ignore

        this.ActiveBufferIndex <- passiveIndex

    member this.AdvanceToNextStateOther() =
        let activeIndex = this.ActiveBufferIndex
        let passiveIndex = (activeIndex + 1) % this.Buffers.Length
        let activeBuffer = this.Buffers[activeIndex]
        let passiveBuffer = this.Buffers[passiveIndex]

        let rows = Array2D.length1 activeBuffer - 2
        let cols = Array2D.length2 activeBuffer - 2

        let degreeOfParallelism = Environment.ProcessorCount
        let totalLength = rows * cols

        Parallel.For(
            0,
            degreeOfParallelism,
            fun workerId ->
                let max = totalLength * (workerId + 1) / degreeOfParallelism
                let startIndex = totalLength * workerId / degreeOfParallelism

                for rowCol in startIndex .. (max - 1) do
                    let row = rowCol / cols + 1
                    let col = rowCol % cols + 1

                    passiveBuffer[row, col] <- ConwayGrid.evolveCellAt row col activeBuffer (activeBuffer[row, col])
        )
        |> ignore

        this.ActiveBufferIndex <- passiveIndex

    member this.AdvanceToNextStateOtherUnsafe() =
        let activeIndex = this.ActiveBufferIndex
        let passiveIndex = (activeIndex + 1) % this.Buffers.Length
        let activeBuffer = this.Buffers[activeIndex]
        let passiveBuffer = this.Buffers[passiveIndex]

        let rows = Array2D.length1 activeBuffer - 2
        let cols = Array2D.length2 activeBuffer - 2

        Parallel.For(
            1,
            rows - 1,
            fun row ->
                for col in 1 .. cols - 2 do
                    passiveBuffer[row, col] <-
                        ConwayGrid.evolveCellAtUnsafe row col activeBuffer (activeBuffer[row, col])
        )
        |> ignore

        this.ActiveBufferIndex <- passiveIndex

    member this.AdvanceToNextStateOtherUnsafeChunk() =
        let activeIndex = this.ActiveBufferIndex
        let passiveIndex = (activeIndex + 1) % this.Buffers.Length
        let activeBuffer = this.Buffers[activeIndex]
        let passiveBuffer = this.Buffers[passiveIndex]

        let rows = Array2D.length1 activeBuffer - 2
        let cols = Array2D.length2 activeBuffer - 2

        let degreeOfParallelism = Environment.ProcessorCount
        let totalLength = rows * cols

        Parallel.For(
            0,
            degreeOfParallelism,
            fun workerId ->
                let max = totalLength * (workerId + 1) / degreeOfParallelism
                let startIndex = totalLength * workerId / degreeOfParallelism

                for rowCol in startIndex .. (max - 1) do
                    let row = rowCol / cols + 1
                    let col = rowCol % cols + 1

                    passiveBuffer[row, col] <-
                        ConwayGrid.evolveCellAtUnsafe row col activeBuffer (activeBuffer[row, col])
        )
        |> ignore

        this.ActiveBufferIndex <- passiveIndex

    [<CompiledName("CreateDead")>]
    static member createDead width height = new ConwayGrid(width, height)

    [<CompiledName("CreateLiving")>]
    static member createLiving width height =
        let initArr =
            Array2D.init (height + 2) (width + 2) (fun i j ->
                if i = 0 || j = 0 || i = height + 1 || j = width + 1 then
                    Cell.dead
                else
                    Cell.living)

        new ConwayGrid(initArr)

    [<CompiledName("CreateRandomWithOdds")>]
    static member createRandomWithOdds width height oddsOfLiving =
        let random = new Random()

        let initArr =
            Array2D.init (height + 2) (width + 2) (fun i j ->
                if i = 0 || j = 0 || i = height + 1 || j = width + 1 then
                    Cell.dead
                else
                    let randomValue = random.Next(0, oddsOfLiving - 1)

                    if randomValue = 0 then Cell.living else Cell.dead)

        new ConwayGrid(initArr)

    [<CompiledName("Init")>]
    static member init width height initializer =
        let initArr =
            Array2D.init (height + 2) (width + 2) (fun i j ->
                if i = 0 || j = 0 || i = height + 1 || j = width + 1 then
                    Cell.dead
                else
                    initializer (i - 1) (j - 1))

        new ConwayGrid(initArr)

    [<CompiledName("InitFromPreset")>]
    static member initFromPreset preset = preset |||> ConwayGrid.init

    [<CompiledName("CountLivingNeighbors")>]
    static member private countLivingNeighbors row col (board: Cell array2d) =
        Convert.ToInt32(Cell.isAlive board.[row - 1, col - 1])
        + Convert.ToInt32(Cell.isAlive board.[row - 1, col])
        + Convert.ToInt32(Cell.isAlive board.[row - 1, col + 1])
        + Convert.ToInt32(Cell.isAlive board.[row, col - 1])
        + Convert.ToInt32(Cell.isAlive board.[row, col + 1])
        + Convert.ToInt32(Cell.isAlive board.[row + 1, col - 1])
        + Convert.ToInt32(Cell.isAlive board.[row + 1, col])
        + Convert.ToInt32(Cell.isAlive board.[row + 1, col + 1])

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    static member inline private fastBoardAccess index (board: Cell array2d) =
        use ptr = fixed &board.[0, 0]
        NativePtr.get ptr index

    [<CompiledName("CountLivingNeighborsUnsafe")>]
    static member private countLivingNeighborsUnsafe row col (board: Cell array2d) =
        let cols = Array2D.length2 board
        let upLeft = (row - 1) * cols + (col - 1)
        let up = (row - 1) * cols + col
        let upRight = (row - 1) * cols + (col + 1)
        let left = row * cols + (col - 1)
        let right = row * cols + (col + 1)
        let downLeft = (row + 1) * cols + (col - 1)
        let down = (row + 1) * cols + col
        let downRight = (row + 1) * cols + (col + 1)

        FastHelpers.retype<CellStatus> (ConwayGrid.fastBoardAccess upLeft board).Status
        + FastHelpers.retype<CellStatus> (ConwayGrid.fastBoardAccess up board).Status
        + FastHelpers.retype<CellStatus> (ConwayGrid.fastBoardAccess upRight board).Status
        + FastHelpers.retype<CellStatus> (ConwayGrid.fastBoardAccess left board).Status
        + FastHelpers.retype<CellStatus> (ConwayGrid.fastBoardAccess right board).Status
        + FastHelpers.retype<CellStatus> (ConwayGrid.fastBoardAccess downLeft board).Status
        + FastHelpers.retype<CellStatus> (ConwayGrid.fastBoardAccess down board).Status
        + FastHelpers.retype<CellStatus> (ConwayGrid.fastBoardAccess downRight board).Status

    [<CompiledName("EvolveCellAt")>]
    static member private evolveCellAt row col board currentCell =
        let livingNeighborsCount = ConwayGrid.countLivingNeighbors row col board

        match livingNeighborsCount with
        | 2 -> Cell.create currentCell.Status
        | 3 -> Cell.living
        | _ -> Cell.dead

    [<CompiledName("EvolveCellAtUnsafe")>]
    static member private evolveCellAtUnsafe row col board currentCell =
        let livingNeighborsCount = ConwayGrid.countLivingNeighborsUnsafe row col board

        match livingNeighborsCount with
        | 2 -> Cell.create currentCell.Status
        | 3 -> Cell.living
        | _ -> Cell.dead
