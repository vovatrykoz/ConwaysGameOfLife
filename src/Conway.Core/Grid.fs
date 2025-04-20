namespace Conway.Core

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
} with

    [<CompiledName("CreateDead")>]
    static member createDead width height =
        let initArr =
            Array2D.init (height + 2) (width + 2) (fun i j ->
                if i = 0 || j = 0 || i = height + 1 || j = width + 1 then
                    BorderCell
                else
                    PlayerCell Cell.dead)

        { Board = initArr; Buffer = initArr }

    [<CompiledName("CreateLiving")>]
    static member createLiving width height =
        let initArr =
            Array2D.init (height + 2) (width + 2) (fun i j ->
                if i = 0 || j = 0 || i = height + 1 || j = width + 1 then
                    BorderCell
                else
                    PlayerCell Cell.living)

        { Board = initArr; Buffer = initArr }

    [<CompiledName("Init")>]
    static member init width height initializer =
        let initArr =
            Array2D.init (height + 2) (width + 2) (fun i j ->
                if i = 0 || j = 0 || i = height + 1 || j = width + 1 then
                    BorderCell
                else
                    PlayerCell(initializer (i - 1) (j - 1)))

        { Board = initArr; Buffer = initArr }

    [<CompiledName("InitFromPreset")>]
    static member initFromPreset preset = preset |||> ConwayGrid.init

    static member board grid = grid.Board

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
        currentCell.Memory.Push currentCell.Status

        match livingNeighborsCount with
        | x when x < 2 -> Cell.create Dead currentCell.Memory
        | x when x = 2 -> Cell.create currentCell.Status currentCell.Memory
        | x when x = 3 -> Cell.create Alive currentCell.Memory
        | _ -> Cell.create Dead currentCell.Memory

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

        grid.Buffer
        |> Array2D.iteri (fun row col value -> grid.Board[row, col] <- value)

        grid

    [<CompiledName("Previous")>]
    static member previous grid =
        let rows = Array2D.length1 grid.Board
        let cols = Array2D.length2 grid.Board

        [
            for row in 0 .. rows - 1 do
                for col in 0 .. cols - 1 ->
                    async {
                        let cell = grid.Board[row, col]

                        match cell with
                        | BorderCell -> return CalculationResult.create row col BorderCell
                        | PlayerCell playerCell ->
                            let previousState =
                                if playerCell.Memory.Count <= 0 then
                                    None
                                else
                                    Some(playerCell.Memory.Pop())

                            match previousState with
                            | None -> return CalculationResult.create row col (PlayerCell playerCell)
                            | Some state ->
                                return
                                    CalculationResult.create
                                        row
                                        col
                                        (PlayerCell {
                                            Status = state
                                            Memory = playerCell.Memory
                                        })
                    }
        ]
        |> Async.Parallel
        |> Async.RunSynchronously
        |> Array.iter (fun calculationResult ->
            grid.Board[calculationResult.Row, calculationResult.Column] <- calculationResult.CellType)

        grid
