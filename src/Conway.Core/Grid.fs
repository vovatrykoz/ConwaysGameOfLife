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

    static member next grid =
        let newBoard =
            grid.Board
            |> Array2D.mapi (fun i j cell ->
                match cell with
                | BorderCell -> BorderCell
                | PlayerCell currentCell ->
                    let neighbors = [
                        grid.Board.[i - 1, j - 1]
                        grid.Board[i - 1, j]
                        grid.Board[i - 1, j + 1]
                        grid.Board.[i, j - 1]
                        grid.Board[i, j + 1]
                        grid.Board.[i + 1, j - 1]
                        grid.Board[i + 1, j]
                        grid.Board[i + 1, j + 1]
                    ]

                    let livingNeighbors =
                        neighbors
                        |> List.choose (fun neighbor ->
                            match neighbor with
                            | BorderCell -> None
                            | PlayerCell cell ->
                                match cell.Status with
                                | Dead -> None
                                | Alive -> Some Alive)

                    match List.length livingNeighbors with
                    | x when x < 2 -> PlayerCell Cell.dead
                    | x when x = 2 ->
                        match currentCell.Status with
                        | Dead -> PlayerCell Cell.dead
                        | Alive -> PlayerCell Cell.living
                    | x when x = 3 -> PlayerCell Cell.living
                    | _ -> PlayerCell Cell.dead)

        { grid with Board = newBoard }
