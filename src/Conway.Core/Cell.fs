namespace Conway.Core

type CellStatus =
    | Alive
    | Dead

type Cell = {
    Status: CellStatus
    Memory: Stack<CellStatus>
} with

    [<CompiledName("CreateLivingCell")>]
    static member living = { Status = Alive; Memory = Stack.empty }

    [<CompiledName("CreateDeadCell")>]
    static member dead = { Status = Dead; Memory = Stack.empty }

    [<CompiledName("IsAlive")>]
    static member isAlive cell = cell.Status = Alive

    [<CompiledName("IsDead")>]
    static member isDead cell = cell.Status = Dead
