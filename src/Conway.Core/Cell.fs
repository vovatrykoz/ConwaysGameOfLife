namespace Conway.Core

type CellStatus =
    | Alive
    | Dead

type Cell = {
    Status: CellStatus
} with

    [<CompiledName("CreateLivingCell")>]
    static member living = { Status = Alive }

    [<CompiledName("CreateDeadCell")>]
    static member dead = { Status = Dead }
