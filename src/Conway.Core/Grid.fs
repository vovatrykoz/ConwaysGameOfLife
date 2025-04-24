namespace Conway.Core

open System
open System.Collections.Generic
open System.Threading.Tasks

[<NoComparison>]
type ConwayGrid = private {
    Buffers: Cell[,][]
    mutable ActiveBufferIndex: int
} with

    member this.Board = this.Buffers[this.ActiveBufferIndex]

    [<CompiledName("CreateDead")>]
    static member createDead width height =
        let initArr = Array2D.init (height + 2) (width + 2) (fun _ _ -> Cell.dead)

        {
            Buffers = [| Array2D.copy initArr; Array2D.copy initArr |]
            ActiveBufferIndex = 0
        }

    [<CompiledName("CreateLiving")>]
    static member createLiving width height =
        let initArr =
            Array2D.init (height + 2) (width + 2) (fun i j ->
                if i = 0 || j = 0 || i = height + 1 || j = width + 1 then
                    Cell.dead
                else
                    Cell.living)

        {
            Buffers = [| Array2D.copy initArr; Array2D.copy initArr |]
            ActiveBufferIndex = 0
        }

    [<CompiledName("CreateRandom")>]
    static member createRandomWithOdds width height oddsOfLiving =
        let random = new Random()

        let initArr =
            Array2D.init (height + 2) (width + 2) (fun i j ->
                if i = 0 || j = 0 || i = height + 1 || j = width + 1 then
                    Cell.dead
                else
                    let randomValue = random.Next(0, oddsOfLiving - 1)

                    if randomValue = 0 then Cell.living else Cell.dead)

        {
            Buffers = [| Array2D.copy initArr; Array2D.copy initArr |]
            ActiveBufferIndex = 0
        }

    [<CompiledName("Init")>]
    static member init width height initializer =
        let initArr =
            Array2D.init (height + 2) (width + 2) (fun i j ->
                if i = 0 || j = 0 || i = height + 1 || j = width + 1 then
                    Cell.dead
                else
                    initializer (i - 1) (j - 1))

        {
            Buffers = [| Array2D.copy initArr; Array2D.copy initArr |]
            ActiveBufferIndex = 0
        }

    [<CompiledName("InitFromPreset")>]
    static member initFromPreset preset = preset |||> ConwayGrid.init

    [<CompiledName("CountLivingNeighbors")>]
    static member private countLivingNeighbors row col (board: Cell array2d) =
        let mutable counter = 0

        counter <- counter + UnsafeUtils.retype<bool, int> (Cell.isAlive board.[row - 1, col - 1])
        counter <- counter + UnsafeUtils.retype<bool, int> (Cell.isAlive board.[row - 1, col])
        counter <- counter + UnsafeUtils.retype<bool, int> (Cell.isAlive board.[row - 1, col + 1])
        counter <- counter + UnsafeUtils.retype<bool, int> (Cell.isAlive board.[row, col - 1])
        counter <- counter + UnsafeUtils.retype<bool, int> (Cell.isAlive board.[row, col + 1])
        counter <- counter + UnsafeUtils.retype<bool, int> (Cell.isAlive board.[row + 1, col - 1])
        counter <- counter + UnsafeUtils.retype<bool, int> (Cell.isAlive board.[row + 1, col])
        counter <- counter + UnsafeUtils.retype<bool, int> (Cell.isAlive board.[row + 1, col + 1])

        counter

    [<CompiledName("ProcessPlayerCell")>]
    static member private processPlayerCell row col currentCell board =
        let livingNeighborsCount = ConwayGrid.countLivingNeighbors row col board

        match livingNeighborsCount with
        | x when x < 2 -> Cell.create Dead
        | x when x = 2 -> Cell.create currentCell.Status
        | x when x = 3 -> Cell.create Alive
        | _ -> Cell.create Dead

    [<CompiledName("Next")>]
    static member next(grid: ConwayGrid) =
        let activeIndex = grid.ActiveBufferIndex
        let passiveIndex = (activeIndex + 1) % grid.Buffers.Length
        let rows = Array2D.length1 grid.Buffers[activeIndex]
        let cols = Array2D.length2 grid.Buffers[activeIndex]

        Parallel.For(
            1,
            rows - 1,
            fun row ->
                for col in 1 .. cols - 2 do
                    grid.Buffers[passiveIndex][row, col] <-
                        ConwayGrid.processPlayerCell
                            row
                            col
                            (grid.Buffers[activeIndex][row, col])
                            grid.Buffers[activeIndex]
        )
        |> ignore

        grid.ActiveBufferIndex <- passiveIndex

        grid
