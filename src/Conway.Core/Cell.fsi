namespace Conway.Core

/// <summary>
/// Represents the possible states of a cell in Conway's Game of Life.
/// </summary>
[<Struct>]
type CellStatus =

    /// <summary>
    /// Indicates that the cell is alive.
    /// </summary>
    | Alive

    /// <summary>
    /// Indicates that the cell is dead.
    /// </summary>
    | Dead

/// <summary>
/// Represents a cell in the Game of Life, containing its current status.
/// </summary>
[<Struct; NoComparison>]
type Cell = {

    /// <summary>
    /// The current status of the cell (alive or dead).
    /// </summary>
    Status: CellStatus
} with

    /// <summary>
    /// Creates a new cell that is alive.
    /// </summary>
    [<CompiledName("Living")>]
    static member living: Cell

    /// <summary>
    /// Creates a new cell that is dead.
    /// </summary>
    [<CompiledName("Dead")>]
    static member dead: Cell

    /// <summary>
    /// Creates a new cell with the given status.
    /// </summary>
    /// <param name="status">The status to assign to the cell.</param>
    /// <returns>A new cell with the specified status.</returns>
    [<CompiledName("Create")>]
    static member create: status: CellStatus -> Cell

    /// <summary>
    /// Determines whether the specified cell is alive.
    /// </summary>
    /// <param name="cell">The cell to check.</param>
    /// <returns><c>true</c> if the cell is alive; otherwise, <c>false</c>.</returns>
    [<CompiledName("IsAlive")>]
    static member inline isAlive: cell: Cell -> bool

    /// <summary>
    /// Determines whether the specified cell is dead.
    /// </summary>
    /// <param name="cell">The cell to check.</param>
    /// <returns><c>true</c> if the cell is dead; otherwise, <c>false</c>.</returns>
    [<CompiledName("IsDead")>]
    static member inline isDead: cell: Cell -> bool
