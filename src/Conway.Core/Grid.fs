namespace Conway.Core

type GridCellType =
    | BorderCell
    | PlayerCell of Cell

type ConwayGrid = {
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

    [<CompiledName("InitFromPreset")>]
    static member initFromPreset preset = preset |||> ConwayGrid.init

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
        |> ConwayGrid.collectNeighbors row col
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
        let livingNeighborsCount = ConwayGrid.countLivingNeighbors row col board

        match livingNeighborsCount with
        | x when x < 2 ->
            PlayerCell {
                Status = Dead
                Memory = currentCell.Memory |> Stack.push currentCell.Status
            }
        | x when x = 2 ->
            PlayerCell {
                Status = currentCell.Status
                Memory = currentCell.Memory |> Stack.push currentCell.Status
            }
        | x when x = 3 ->
            PlayerCell {
                Status = Alive
                Memory = currentCell.Memory |> Stack.push currentCell.Status
            }
        | _ ->
            PlayerCell {
                Status = Dead
                Memory = currentCell.Memory |> Stack.push currentCell.Status
            }

    [<CompiledName("Next")>]
    static member next(grid: ConwayGrid) =
        let rows = Array2D.length1 grid.Board
        let cols = Array2D.length2 grid.Board

        let gridProcess = [
            for row in 0 .. rows - 1 do
                for col in 0 .. cols - 1 ->
                    async {
                        let cell = grid.Board[row, col]

                        match cell with
                        | BorderCell -> return (row, col, BorderCell)
                        | PlayerCell playerCell ->
                            return (row, col, ConwayGrid.processPlayerCell row col playerCell grid.Board)
                    }
        ]

        gridProcess
        |> Async.Parallel
        |> Async.RunSynchronously
        |> Array.iter (fun (row, col, result) -> grid.Board[row, col] <- result)

    [<CompiledName("Previous")>]
    static member previous grid =
        let newBoard =
            grid.Board
            |> Array2D.map (fun cell ->
                task {
                    match cell with
                    | BorderCell -> return BorderCell
                    | PlayerCell playerCell ->
                        let previousState, otherMemories = Stack.tryPop playerCell.Memory

                        match previousState with
                        | None -> return PlayerCell playerCell
                        | Some state ->
                            return
                                PlayerCell {
                                    Status = state
                                    Memory = otherMemories
                                }
                })
            |> Array2D.map (fun task -> task.Result)

        { grid with Board = newBoard }
