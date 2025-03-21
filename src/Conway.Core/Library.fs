namespace Conway.Core

type CellStatus =
    | Alive
    | Dead

type Cell = {
    Status: CellStatus
} with

    static member createLivingCell = { Status = Alive }
