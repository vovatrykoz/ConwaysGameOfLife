namespace Conway.Core

open System.Collections.Generic

[<Struct; NoComparison>]
type GridCellType =
    | BorderCell
    | PlayerCell of Cell

[<Struct; NoComparison>]
type private CalculationResult = {
    Row: int
    Column: int
    CellType: GridCellType
} with

    static member create row col cellType = {
        Row = row
        Column = col
        CellType = cellType
    }

[<NoComparison>]
type ConwayGrid = private {
    Board: GridCellType[,]
    Buffer: GridCellType[,]
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

        { Board = initArr (); Buffer = initArr (); Memory = new Stack<GridCellType[,]>() }

    [<CompiledName("CreateLiving")>]
    static member createLiving width height =
        let initArr () =
            Array2D.init (height + 2) (width + 2) (fun i j ->
                if i = 0 || j = 0 || i = height + 1 || j = width + 1 then
                    BorderCell
                else
                    PlayerCell Cell.living)

        { Board = initArr (); Buffer = initArr (); Memory = new Stack<GridCellType[,]>() }

    [<CompiledName("Init")>]
    static member init width height initializer =
        let initArr () =
            Array2D.init (height + 2) (width + 2) (fun i j ->
                if i = 0 || j = 0 || i = height + 1 || j = width + 1 then
                    BorderCell
                else
                    PlayerCell(initializer (i - 1) (j - 1)))

        { Board = initArr (); Buffer = initArr (); Memory = new Stack<GridCellType[,]>()  }

    [<CompiledName("InitFromPreset")>]
    static member initFromPreset preset = preset |||> ConwayGrid.init

    static member board grid = grid.Board

    [<CompiledName("CollectNeighbors")>]
    static member private collectNeighbors row col (board: GridCellType array2d) = [|
        board.[row - 1, col - 1]
        board.[row - 1, col]
        board.[row - 1, col + 1]
        board.[row, col - 1]
        board.[row, col + 1]
        board.[row + 1, col - 1]
        board.[row + 1, col]
        board.[row + 1, col + 1]
    |]

    [<CompiledName("CountLivingNeighbors")>]
    static member private countLivingNeighbors row col board =
        board
        |> ConwayGrid.collectNeighbors row col
        |> Array.fold (fun acc neighbor ->
            match neighbor with
            | BorderCell -> acc
            | PlayerCell cell ->
                match cell.Status with
                | Dead -> acc
                | Alive -> acc + 1) 0

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
        let rows = Array2D.length1 grid.Board
        let cols = Array2D.length2 grid.Board

        for row in 0 .. rows - 1 do
            for col in 0 .. cols - 1 do
                grid.Buffer[row, col] <-
                    match grid.Board[row, col] with
                    | BorderCell -> BorderCell
                    | PlayerCell playerCell -> PlayerCell (ConwayGrid.processPlayerCell row col playerCell grid.Board)

        for row in 0 .. rows - 1 do
            for col in 0 .. cols - 1 do
                grid.Board[row, col] <- grid.Buffer[row, col]

        grid

    [<CompiledName("Previous")>]
    static member previous grid =
        let rows = Array2D.length1 grid.Board
        let cols = Array2D.length2 grid.Board

        if grid.Memory.Count <= 0 then
            grid
        else
            let previousState = grid.Memory.Pop();

            for row in 0 .. rows - 1 do
                for col in 0 .. cols - 1 do
                    grid.Board[row, col] <- previousState[row, col]


            grid
