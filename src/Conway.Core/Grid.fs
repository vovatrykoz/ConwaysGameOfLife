namespace Conway.Core

open Microsoft.FSharp.NativeInterop
open System
open System.Runtime.CompilerServices
open System.Threading.Tasks

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

type ConwayGrid private (startingGrid: int<CellStatus> array2d) =

    private new(width: int, height: int) =
        let initArr = Array2D.create (height + 2) (width + 2) 0<CellStatus>

        new ConwayGrid(initArr)

    member val private Buffers = [| Array2D.copy startingGrid; Array2D.copy startingGrid |] with get, set

    member val private ActiveBufferIndex = 0 with get, set

    member this.Board = this.Buffers[this.ActiveBufferIndex]

    member this.ActiveWidth = Array2D.length2 this.Board - 2

    member this.ActiveHeight = Array2D.length1 this.Board - 2

    [<CompiledName("CountLivingNeighbors"); MethodImpl(MethodImplOptions.AggressiveInlining)>]
    static member inline private countLivingNeighbors
        rowAbove
        rowCurrent
        rowBelow
        colCurrent
        (ptr: nativeptr<int<CellStatus>>)
        =
        NativePtr.get ptr (rowAbove + (colCurrent - 1))
        + NativePtr.get ptr (rowAbove + colCurrent)
        + NativePtr.get ptr (rowAbove + (colCurrent + 1))
        + NativePtr.get ptr (rowCurrent + (colCurrent - 1))
        + NativePtr.get ptr (rowCurrent + (colCurrent + 1))
        + NativePtr.get ptr (rowBelow + (colCurrent - 1))
        + NativePtr.get ptr (rowBelow + colCurrent)
        + NativePtr.get ptr (rowBelow + (colCurrent + 1))

    [<CompiledName("EvolveCellAt"); MethodImpl(MethodImplOptions.AggressiveInlining)>]
    static member inline private evolveCellAt row col cols activePtr passivePtr =
        let rowAbove = (row - 1) * cols
        let rowCurrent = row * cols
        let rowBelow = (row + 1) * cols
        let index = row * cols + col

        let livingNeighborsCount =
            ConwayGrid.countLivingNeighbors rowAbove rowCurrent rowBelow col activePtr

        match livingNeighborsCount with
        | 2<Neighbors> ->
            let currentValue = NativePtr.get activePtr index
            NativePtr.set passivePtr index currentValue
        | 3<Neighbors> -> NativePtr.set passivePtr index 1<CellStatus>
        | _ -> NativePtr.set passivePtr index 0<CellStatus>

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
                    0<CellStatus>
                else
                    1<CellStatus>)

        new ConwayGrid(initArr)

    [<CompiledName("CreateRandomWithOdds")>]
    static member createRandomWithOdds width height oddsOfLiving =
        let random = new Random()

        let initArr =
            Array2D.init (height + 2) (width + 2) (fun i j ->
                if i = 0 || j = 0 || i = height + 1 || j = width + 1 then
                    0<CellStatus>
                else
                    let randomValue = random.Next(0, oddsOfLiving - 1)

                    if randomValue = 0 then 1<CellStatus> else 0<CellStatus>)

        new ConwayGrid(initArr)

    [<CompiledName("Init")>]
    static member init width height initializer =
        let initArr =
            Array2D.init (height + 2) (width + 2) (fun i j ->
                if i = 0 || j = 0 || i = height + 1 || j = width + 1 then
                    0<CellStatus>
                else
                    initializer (i - 1) (j - 1))

        new ConwayGrid(initArr)

    [<CompiledName("CopyFrom")>]
    static member copyFrom(otherGrid: ConwayGrid) =
        new ConwayGrid(Array2D.copy otherGrid.Board)
