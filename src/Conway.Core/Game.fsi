namespace Conway.Core

/// <summary>
/// Represents the mode in which the game can run.
/// </summary>
type GameMode =
    /// <summary>Run the game indefinitely.</summary>
    | Infinite
    /// <summary>Run the game for a limited number of steps.</summary>
    | Limited of int
    /// <summary>Pause the game.</summary>
    | Paused

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
    /// Gets or sets the current state of the game grid.
    /// </summary>
    member State: ConwayGrid with get, set

    /// <summary>
    /// Gets the current generation number.
    /// </summary>
    member Generation: int with get

    /// <summary>
    /// Starts running the game using the specified game mode.
    /// </summary>
    /// <param name="mode">The mode in which to run the game.</param>
    member Run: GameMode -> unit

    /// <summary>
    /// Advances the game by one generation step.
    /// </summary>
    member RunOneStep: unit -> unit

    /// <summary>
    /// Clears the game's history and resets the generation counter.
    /// </summary>
    member ClearHistory: unit -> unit
