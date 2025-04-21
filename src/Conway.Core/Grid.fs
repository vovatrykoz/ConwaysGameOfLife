namespace Conway.Core

open System.Collections.Generic
open System.Threading.Tasks

[<Struct; NoComparison>]
type GridCellType =
    | BorderCell
    | PlayerCell of Cell

[<NoComparison>]
type ConwayGrid = private {
    Buffers: GridCellType[,][]
    mutable ActiveBufferIndex: int
    Memory: Stack<GridCellType[,]>
} with

    [<CompiledName("CreateDead")>]
    static member createDead width height =
        let initArr () =
            Array2D.init (height + 2) (width + 2) (fun i j ->
                if i = 0 || j = 0 || i = height + 1 || j = width + 1 then
                    BorderCell
                else
                    PlayerCell Cell.dead)

        {
            Buffers = [| initArr (); initArr () |]
            ActiveBufferIndex = 0
            Memory = new Stack<GridCellType[,]>()
        }

    [<CompiledName("CreateLiving")>]
    static member createLiving width height =
        let initArr () =
            Array2D.init (height + 2) (width + 2) (fun i j ->
                if i = 0 || j = 0 || i = height + 1 || j = width + 1 then
                    BorderCell
                else
                    PlayerCell Cell.living)

        {
            Buffers = [| initArr (); initArr () |]
            ActiveBufferIndex = 0
            Memory = new Stack<GridCellType[,]>()
        }

    [<CompiledName("Init")>]
    static member init width height initializer =
        let initArr () =
            Array2D.init (height + 2) (width + 2) (fun i j ->
                if i = 0 || j = 0 || i = height + 1 || j = width + 1 then
                    BorderCell
                else
                    PlayerCell(initializer (i - 1) (j - 1)))

        {
            Buffers = [| initArr (); initArr () |]
            ActiveBufferIndex = 0
            Memory = new Stack<GridCellType[,]>()
        }

    [<CompiledName("InitFromPreset")>]
    static member initFromPreset preset = preset |||> ConwayGrid.init

    static member board grid = grid.Buffers[grid.ActiveBufferIndex]

    [<CompiledName("CountLivingNeighbors")>]
    static member private countLivingNeighbors row col (board: GridCellType array2d) =
        let mutable counter = 0

        counter <- counter + (
            match board.[row - 1, col - 1] with
            | BorderCell -> 0
            | PlayerCell cell -> 
                match cell.Status with
                | Dead -> 0
                | Alive -> 1
            )

        counter <- counter + (
            match board.[row - 1, col] with
            | BorderCell -> 0
            | PlayerCell cell -> 
                match cell.Status with
                | Dead -> 0
                | Alive -> 1
            )

        counter <- counter + (
            match board.[row - 1, col + 1] with
            | BorderCell -> 0
            | PlayerCell cell -> 
                match cell.Status with
                | Dead -> 0
                | Alive -> 1
            )

        counter <- counter + (
            match board.[row, col + 1] with
            | BorderCell -> 0
            | PlayerCell cell -> 
                match cell.Status with
                | Dead -> 0
                | Alive -> 1
            )

        counter <- counter + (
            match board.[row, col - 1] with
            | BorderCell -> 0
            | PlayerCell cell -> 
                match cell.Status with
                | Dead -> 0
                | Alive -> 1
            )

        counter <- counter + (
            match board.[row + 1, col - 1] with
            | BorderCell -> 0
            | PlayerCell cell -> 
                match cell.Status with
                | Dead -> 0
                | Alive -> 1
            )

        counter <- counter + (
            match board.[row + 1, col] with
            | BorderCell -> 0
            | PlayerCell cell -> 
                match cell.Status with
                | Dead -> 0
                | Alive -> 1
            )

        counter <- counter + (
            match board.[row + 1, col + 1] with
            | BorderCell -> 0
            | PlayerCell cell -> 
                match cell.Status with
                | Dead -> 0
                | Alive -> 1
            )

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
                        match grid.Buffers[activeIndex][row, col] with
                        | BorderCell -> BorderCell
                        | PlayerCell playerCell ->
                            PlayerCell(ConwayGrid.processPlayerCell row col playerCell grid.Buffers[activeIndex])
        )
        |> ignore

        grid.ActiveBufferIndex <- passiveIndex

        grid

    [<CompiledName("Previous")>]
    static member previous grid =
        let rows = Array2D.length1 grid.Buffers[grid.ActiveBufferIndex]
        let cols = Array2D.length2 grid.Buffers[grid.ActiveBufferIndex]

        if grid.Memory.Count <= 0 then
            grid
        else
            let previousState = grid.Memory.Pop()

            for row in 0 .. rows - 1 do
                for col in 0 .. cols - 1 do
                    grid.Buffers[grid.ActiveBufferIndex][row, col] <- previousState[row, col]

            grid
