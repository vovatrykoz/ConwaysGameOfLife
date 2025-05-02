namespace Conway.Core

[<Measure>]
type cellStatus

[<Struct>]
type CellStatus =
    | Alive
    | Dead

[<Struct; NoComparison>]
type Cell = {
    mutable Status: int<cellStatus>
} with

    [<CompiledName("Living")>]
    static member living = { Status = 1<cellStatus> }

    [<CompiledName("Dead")>]
    static member dead = { Status = 0<cellStatus> }

    [<CompiledName("Create")>]
    static member create status = { Status = status }

    [<CompiledName("IsAlive")>]
    static member inline isAlive cell = cell.Status = 1<cellStatus>

    [<CompiledName("IsDead")>]
    static member inline isDead cell = cell.Status = 0<cellStatus>
