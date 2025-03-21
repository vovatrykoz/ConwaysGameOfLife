namespace Conway.Core

type GridCellType =
    | BorderCell
    | PlayerCell of Cell

type Grid = {
    Board: GridCellType[,]
} with

    static member createDead width height = {
        Board =
            Array2D.init (height + 2) (width + 2) (fun i j ->
                if i = 0 || j = 0 || i = height + 1 || j = width + 1 then
                    BorderCell
                else
                    PlayerCell Cell.createDeadCell)
    }

    static member createLiving width height = {
        Board =
            Array2D.init (height + 2) (width + 2) (fun i j ->
                if i = 0 || j = 0 || i = height + 1 || j = width + 1 then
                    BorderCell
                else
                    PlayerCell Cell.createLivingCell)
    }

    static member init width height initializer = {
        Board =
            Array2D.init (height + 2) (width + 2) (fun i j ->
                if i = 0 || j = 0 || i = height + 1 || j = width + 1 then
                    BorderCell
                else
                    PlayerCell(initializer (i + 1) (j + 1)))
    }
