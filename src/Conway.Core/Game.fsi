namespace Conway.Core

/// <summary>
/// Represents the mode in which the game can run.
/// </summary>
[<Measure>]
type GameMode

/// <summary>
/// Represents the Game of Life engine, maintaining state and progression logic.
/// </summary>
[<Class>]
type Game =
    /// <summary>
    /// Initializes a new instance of the Game class with the given initial grid state.
    /// </summary>
    /// <param name="initialState">The starting state of the game grid.</param>
    new: initialState: ConwayGrid -> Game

    /// <summary>
    /// Initializes a new instance of the Game class with the given initial grid state and a generation.
    /// </summary>
    /// <param name="initialState">The starting state of the game grid.</param>
    new: initialState: ConwayGrid * generation: int -> Game

    /// <summary>
    /// Gets or sets the current state of the game grid.
    /// </summary>
    member CurrentState: ConwayGrid with get, set

    /// <summary>
    /// Gets the initial state of the grid.
    /// </summary>
    member InitialState: ConwayGrid with get

    /// <summary>
    /// Gets the current generation number.
    /// </summary>
    member Generation: int with get

    /// <summary>
    /// Starts running the game using the specified game mode.
    /// </summary>
    /// <param name="mode">The mode in which to run the game.</param>
    member Run: int<GameMode> -> unit

    /// <summary>
    /// Advances the game by one generation step.
    /// </summary>
    member RunOneStep: unit -> unit

    /// <summary>
    /// Clears the game's history and resets the generation counter.
    /// </summary>
    member ResetState: unit -> unit

    /// <summary>
    /// Clears the game's history and remembers the current state as the "clear" state.
    /// </summary>
    member ResetGenerationCounter: unit -> unit

    [<CompiledName("CreateFrom")>]
    static member createFrom: currentState: ConwayGrid -> initialState: ConwayGrid -> generationCounter: int -> Game
