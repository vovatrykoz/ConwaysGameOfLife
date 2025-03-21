namespace Conway.Core

type CellStatus =
    | Alive
    | Dead

type Cell = {
    Status: CellStatus
} with

    [<CompiledName("CreateLivingCell")>]
    static member createLivingCell = { Status = Alive }

    [<CompiledName("CreateDeadCell")>]
    static member createDeadCell = { Status = Dead }
