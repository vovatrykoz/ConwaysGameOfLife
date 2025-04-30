namespace Conway.Core

/// <summary>
/// Represents the 2D grid of cells used in Conway's Game of Life.
/// Provides methods for initialization and advancing the simulation.
/// </summary>
[<Class>]
type ConwayGrid =
    /// <summary>
    /// Initializes a new ConwayGrid with a predefined 2D array of cells.
    /// </summary>
    /// <param name="startingGrid">A 2D array representing the initial state of the grid.</param>
    private new: startingGrid: Cell array2d -> ConwayGrid

    /// <summary>
    /// Initializes a new ConwayGrid with the specified dimensions. All cells are initially dead.
    /// </summary>
    /// <param name="width">The number of columns in the grid.</param>
    /// <param name="height">The number of rows in the grid.</param>
    private new: width: int * height: int -> ConwayGrid

    /// <summary>
    /// Gets the current state of the board as a 2D array of cells.
    /// </summary>
    member Board: Cell array2d

    /// <summary>
    /// Advances the grid to the next state according to Conway's Game of Life rules.
    /// </summary>
    member AdvanceToNextState: unit -> unit

    /// <summary>
    /// Advances the grid to the next state according to Conway's Game of Life rules.
    /// </summary>
    member AdvanceToNextStateOther: unit -> unit

    /// <summary>
    /// Advances the grid to the next state according to Conway's Game of Life rules.
    /// </summary>
    member AdvanceToNextStateOtherUnsafe: unit -> unit

    /// <summary>
    /// Advances the grid to the next state according to Conway's Game of Life rules.
    /// </summary>
    member AdvanceToNextStateOtherUnsafeChunk: unit -> unit

    /// <summary>
    /// Creates a grid with all cells dead.
    /// </summary>
    /// <param name="width">Grid width.</param>
    /// <param name="height">Grid height.</param>
    [<CompiledName("CreateDead")>]
    static member createDead: width: int -> height: int -> ConwayGrid

    /// <summary>
    /// Creates a grid with all cells alive.
    /// </summary>
    /// <param name="width">Grid width.</param>
    /// <param name="height">Grid height.</param>
    [<CompiledName("CreateLiving")>]
    static member createLiving: width: int -> height: int -> ConwayGrid

    /// <summary>
    /// Creates a grid with cells randomly set to alive based on the specified odds.
    /// </summary>
    /// <param name="width">Grid width.</param>
    /// <param name="height">Grid height.</param>
    /// <param name="oddsOfLiving">Represents 1-in-x odds of a cell survining.</param>
    [<CompiledName("CreateRandomWithOdds")>]
    static member createRandomWithOdds: width: int -> height: int -> oddsOfLiving: int -> ConwayGrid

    /// <summary>
    /// Initializes a grid using a custom initializer function for each cell.
    /// </summary>
    /// <param name="width">Grid width.</param>
    /// <param name="height">Grid height.</param>
    /// <param name="initializer">Function to determine the initial value of each cell.</param>
    [<CompiledName("Init")>]
    static member init: width: int -> height: int -> initializer: (int -> int -> Cell) -> ConwayGrid

    /// <summary>
    /// Initializes a grid from a preset containing dimensions and an initializer function.
    /// </summary>
    /// <param name="preset">A tuple of width, height, and initializer function.</param>
    [<CompiledName("InitFromPreset")>]
    static member initFromPreset: preset: (int * int * (int -> int -> Cell)) -> ConwayGrid
