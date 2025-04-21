namespace Conway.Core

open System.Collections.Generic

[<Struct>]
type CellStatus =
    | Alive
    | Dead

[<Struct; NoComparison>]
type Cell = {
    Status: CellStatus
} with

    [<CompiledName("Living")>]
    static member living = {
        Status = Alive
    }

    [<CompiledName("Dead")>]
    static member dead = {
        Status = Dead
    }

    [<CompiledName("Create")>]
    static member create status = { Status = status }

    [<CompiledName("IsAlive")>]
    static member isAlive cell = cell.Status = Alive

    [<CompiledName("IsDead")>]
    static member isDead cell = cell.Status = Dead
