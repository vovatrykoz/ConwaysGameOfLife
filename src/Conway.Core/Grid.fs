namespace Conway.Core

open Microsoft.FSharp.NativeInterop
open System
open System.Runtime.CompilerServices
open System.Threading.Tasks
open System.Runtime.Intrinsics
open System.Runtime.Intrinsics.X86

// Uses of this construct may result in the generation of unverifiable .NET IL code.
// This warning can be disabled using '--nowarn:9' or '#nowarn "9"'
#nowarn "9" // Warning appears due to the use of "NativePtr.get" and the board pointer down below

[<Measure>]
type CellStatus

// This is just to improve the readability of the match ... with expression in the evolveCellAt member
// Shouldn't really be used outside of that part or similar match ... with expressions
// CellStatus should always be preferred
[<Measure>]
type Neighbors = CellStatus

module private Constants =

    [<Literal>]
    let deadCell = 0<CellStatus>

    [<Literal>]
    let livingCell = 1<CellStatus>

type ConwayGrid private (startingGrid: int<CellStatus> array2d) =

    private new(width: int, height: int) =
        let initArr = Array2D.create (height + 2) (width + 2) 0<CellStatus>

        new ConwayGrid(initArr)

    static member DeadCell = Constants.deadCell

    static member LivingCell = Constants.livingCell

    member val private Buffers = [| Array2D.copy startingGrid; Array2D.copy startingGrid |] with get, set

    member val private ActiveBufferIndex = 0 with get, set

    member this.Board = this.Buffers[this.ActiveBufferIndex]

    member this.ActiveWidth = Array2D.length2 this.Board - 2

    member this.ActiveHeight = Array2D.length1 this.Board - 2

    /// <summary>
    /// Counts the number of living neighbor cells surrounding a given cell in the Conway grid.
    /// </summary>
    /// <param name="topIndex">The linear index of the cell directly above the current cell.</param>
    /// <param name="index">The linear index of the current cell.</param>
    /// <param name="bottomIndex">The linear index of the cell directly below the current cell.</param>
    /// <param name="ptr">A native pointer to the contiguous memory block representing the grid, where each cell’s status is stored as an integer.</param>
    /// <returns>
    /// The number of living neighbors surrounding the given cell, as an integer sum of adjacent cell statuses.
    /// </returns>
    [<CompiledName("CountLivingNeighbors"); MethodImpl(MethodImplOptions.AggressiveOptimization)>]
    static member inline private countLivingNeighbors topIndex index bottomIndex (ptr: nativeptr<int<CellStatus>>) =
        NativePtr.get ptr (topIndex - 1)
        + NativePtr.get ptr topIndex
        + NativePtr.get ptr (topIndex + 1)
        + NativePtr.get ptr (index - 1)
        + NativePtr.get ptr (index + 1)
        + NativePtr.get ptr (bottomIndex - 1)
        + NativePtr.get ptr bottomIndex
        + NativePtr.get ptr (bottomIndex + 1)

    /// <summary>
    /// Evolves a single cell in the grid according to the rules of Conway’s Game of Life.
    /// </summary>
    /// <param name="row">The row index of the cell to evolve.</param>
    /// <param name="col">The column index of the cell to evolve.</param>
    /// <param name="cols">The total number of columns in the grid.</param>
    /// <param name="activePtr">A native pointer to the active grid buffer representing the current state.</param>
    /// <param name="passivePtr">A native pointer to the passive grid buffer where the next generation state will be written.</param>
    /// <remarks>
    /// The function computes the number of living neighbors and applies Conway’s rules:
    /// - A cell with 2 living neighbors remains in its current state.
    /// - A cell with 3 living neighbors becomes (or remains) alive.
    /// - All other cells become (or remain) dead.
    /// </remarks>
    [<CompiledName("EvolveCellAt"); MethodImpl(MethodImplOptions.AggressiveOptimization)>]
    static member inline private evolveCellAt row col cols activePtr passivePtr =
        let rowAbove = (row - 1) * cols
        let rowCurrent = row * cols
        let rowBelow = (row + 1) * cols
        let index = rowCurrent + col
        let topIndex = rowAbove + col
        let bottomIndex = rowBelow + col

        let livingNeighborsCount =
            ConwayGrid.countLivingNeighbors topIndex index bottomIndex activePtr

        match livingNeighborsCount with
        | 2<Neighbors> ->
            let currentValue = NativePtr.get activePtr index
            NativePtr.set passivePtr index currentValue
        | 3<Neighbors> -> NativePtr.set passivePtr index Constants.livingCell
        | _ -> NativePtr.set passivePtr index Constants.deadCell

    [<MethodImpl(MethodImplOptions.AggressiveOptimization)>]
    member this.AdvanceToNextState() =
        let activeIndex = this.ActiveBufferIndex
        let passiveIndex = (activeIndex + 1) % this.Buffers.Length
        let activeBuffer = this.Buffers[activeIndex]
        let passiveBuffer = this.Buffers[passiveIndex]

        let rows = Array2D.length1 activeBuffer
        let cols = Array2D.length2 activeBuffer
        let lastCol = cols - 2

        use activePtr = fixed &activeBuffer.[0, 0]
        use passivePtr = fixed &passiveBuffer.[0, 0]

        Parallel.For(
            1,
            rows - 1,
            fun row ->
                for col in 1..lastCol do
                    ConwayGrid.evolveCellAt row col cols activePtr passivePtr
        )
        |> ignore

        this.ActiveBufferIndex <- passiveIndex

    [<CompiledName("CreateDead")>]
    static member createDead width height = new ConwayGrid(width, height)

    [<CompiledName("CreateLiving")>]
    static member createLiving width height =
        let initArr =
            Array2D.init (height + 2) (width + 2) (fun i j ->
                if i = 0 || j = 0 || i = height + 1 || j = width + 1 then
                    Constants.deadCell
                else
                    Constants.livingCell)

        new ConwayGrid(initArr)

    [<CompiledName("CreateRandomWithOdds")>]
    static member createRandomWithOdds width height oddsOfLiving =
        let random = new Random()

        let initArr =
            Array2D.init (height + 2) (width + 2) (fun i j ->
                if i = 0 || j = 0 || i = height + 1 || j = width + 1 then
                    Constants.deadCell
                else
                    let randomValue = random.Next(0, oddsOfLiving - 1)

                    if randomValue = 0 then
                        Constants.livingCell
                    else
                        Constants.deadCell)

        new ConwayGrid(initArr)

    [<CompiledName("Init")>]
    static member init width height initializer =
        let initArr =
            Array2D.init (height + 2) (width + 2) (fun i j ->
                if i = 0 || j = 0 || i = height + 1 || j = width + 1 then
                    Constants.deadCell
                else
                    initializer (i - 1) (j - 1))

        new ConwayGrid(initArr)

    [<CompiledName("CopyFrom")>]
    static member copyFrom(otherGrid: ConwayGrid) =
        new ConwayGrid(Array2D.copy otherGrid.Board)
