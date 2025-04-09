namespace Conway.Core

type GridCellType =
    | BorderCell
    | PlayerCell of Cell

type Grid = {
    Board: GridCellType[,]
} with

    [<CompiledName("CreateDead")>]
    static member createDead width height = {
        Board =
            Array2D.init (height + 2) (width + 2) (fun i j ->
                if i = 0 || j = 0 || i = height + 1 || j = width + 1 then
                    BorderCell
                else
                    PlayerCell Cell.dead)
    }

    [<CompiledName("CreateLiving")>]
    static member createLiving width height = {
        Board =
            Array2D.init (height + 2) (width + 2) (fun i j ->
                if i = 0 || j = 0 || i = height + 1 || j = width + 1 then
                    BorderCell
                else
                    PlayerCell Cell.living)
    }

    [<CompiledName("Init")>]
    static member init width height initializer = {
        Board =
            Array2D.init (height + 2) (width + 2) (fun i j ->
                if i = 0 || j = 0 || i = height + 1 || j = width + 1 then
                    BorderCell
                else
                    PlayerCell(initializer (i - 1) (j - 1)))
    }

    [<CompiledName("Init")>]
    static member initFromPreset preset = preset |||> Grid.init

    [<CompiledName("CollectNeighbors")>]
    static member private collectNeighbors row col (board: GridCellType array2d) = [
        board.[row - 1, col - 1]
        board.[row - 1, col]
        board.[row - 1, col + 1]
        board.[row, col - 1]
        board.[row, col + 1]
        board.[row + 1, col - 1]
        board.[row + 1, col]
        board.[row + 1, col + 1]
    ]

    [<CompiledName("CountLivingNeighbors")>]
    static member private countLivingNeighbors row col (board: GridCellType array2d) =
        board
        |> Grid.collectNeighbors row col
        |> List.filter (fun neighbor ->
            match neighbor with
            | BorderCell -> false
            | PlayerCell cell ->
                match cell.Status with
                | Dead -> false
                | Alive -> true)
        |> List.length

    [<CompiledName("ProcessPlayerCell")>]
    static member private processPlayerCell row col currentCell (board: GridCellType array2d) =
        let livingNeighborsCount = Grid.countLivingNeighbors row col board

        match livingNeighborsCount with
        | x when x < 2 -> PlayerCell Cell.dead
        | x when x = 2 -> PlayerCell currentCell
        | x when x = 3 -> PlayerCell Cell.living
        | _ -> PlayerCell Cell.dead

    [<CompiledName("Next")>]
    static member next grid =
        let newBoard =
            grid.Board
            |> Array2D.mapi (fun row col cell ->
                task {
                    match cell with
                    | BorderCell -> return BorderCell
                    | PlayerCell playerCell -> return Grid.processPlayerCell row col playerCell grid.Board
                })
            |> Array2D.map (fun task -> task.Result)

        { grid with Board = newBoard }
