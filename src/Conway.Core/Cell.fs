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

    [<CompiledName("IsAlive")>]
    static member isAlive cell = cell.Status = Alive

    [<CompiledName("IsDead")>]
    static member isDead cell = cell.Status = Dead
