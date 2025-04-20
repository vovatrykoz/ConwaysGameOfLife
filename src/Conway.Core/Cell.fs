namespace Conway.Core

[<Struct>]
type CellStatus =
    | Alive
    | Dead

[<Struct>]
type Cell = {
    Status: CellStatus
    Memory: Stack<CellStatus>
} with

    [<CompiledName("Living")>]
    static member living = { Status = Alive; Memory = Stack.empty }

    [<CompiledName("Dead")>]
    static member dead = { Status = Dead; Memory = Stack.empty }

    [<CompiledName("Create")>]
    static member create status memory = { Status = status; Memory = memory }

    [<CompiledName("IsAlive")>]
    static member isAlive cell = cell.Status = Alive

    [<CompiledName("IsDead")>]
    static member isDead cell = cell.Status = Dead
