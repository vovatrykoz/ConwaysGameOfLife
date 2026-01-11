namespace Conway.Core

type GameState =
    | Paused = 0
    | Step = 1
    | Infinite = 2

[<Sealed>]
type Game
    private (initialState: ConwayGrid, startingGrid: int<CellStatus> array2d, generation: int, startingGeneration: int)
    =

    let mutable _internalState = initialState
    let mutable _initialState = startingGrid
    let mutable _generation = generation
    let mutable _initialGeneration = startingGeneration

    new(initialState: ConwayGrid, generation: int) =
        let board = Array2D.copy initialState.Board
        Game(initialState, board, generation, generation)

    new(initialState: ConwayGrid) = Game(initialState, 1)

    member _.CurrentState
        with get () = _internalState
        and set newState =
            _internalState <- newState
            _initialState <- Array2D.copy newState.Board
            _generation <- 1
            _initialGeneration <- _generation

    member _.StartingGrid
        with get () = _initialState
        and private set newValue = _initialState <- newValue

    member _.Generation
        with get () = _generation
        and private set newValue = _generation <- newValue

    member _.StartingGeneration
        with get () = _initialGeneration
        and private set newValue = _initialGeneration <- newValue

    member this.RunOneStep() =
        this.CurrentState.AdvanceToNextState()
        this.Generation <- this.Generation + 1

    member _.ResetState() =
        _internalState <- new ConwayGrid(_initialState)
        _generation <- _initialGeneration

    member this.ResetGenerationCounter() =
        _initialState <- Array2D.copy this.CurrentState.Board
        _generation <- 1
        _initialGeneration <- _generation

    [<CompiledName("CreateFrom")>]
    static member createFrom
        (
            currentState: ConwayGrid,
            initialState: int<CellStatus> array2d,
            generationCounter: int,
            startingGeneration: int
        ) =

        let activeWidth = currentState.ActiveWidth
        let activeHeight = currentState.ActiveHeight

        let initWidth = Array2D.length2 initialState
        let initHeight = Array2D.length1 initialState

        if activeWidth <> initWidth || activeHeight <> initHeight then
            invalidArg
                (nameof initialState)
                $"Expected (w:{activeWidth}, h:{activeHeight}), got (w:{initWidth}, h:{initHeight})"

        let padded =
            Array2D.init (activeHeight + 2) (activeWidth + 2) (fun i j ->
                if i = 0 || j = 0 || i = activeHeight + 1 || j = activeWidth + 1 then
                    ConwayGrid.DeadCell
                else
                    initialState.[i - 1, j - 1])

        let stateCopy = ConwayGrid.copyFrom currentState

        Game(stateCopy, padded, generationCounter, startingGeneration)
