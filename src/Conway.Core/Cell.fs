namespace Conway.Core

open System.Collections.Generic

[<Struct>]
type CellStatus =
    | Alive
    | Dead

[<Struct; NoComparison>]
type Cell = {
    Status: CellStatus
    Memory: Stack<CellStatus>
} with

    [<CompiledName("Living")>]
    static member living = {
        Status = Alive
        Memory = new Stack<CellStatus>()
    }

    [<CompiledName("Dead")>]
    static member dead = {
        Status = Dead
        Memory = new Stack<CellStatus>()
    }

    [<CompiledName("Create")>]
    static member create status memory = { Status = status; Memory = memory }

    [<CompiledName("IsAlive")>]
    static member isAlive cell = cell.Status = Alive

    [<CompiledName("IsDead")>]
    static member isDead cell = cell.Status = Dead
