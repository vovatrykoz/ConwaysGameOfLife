namespace Conway.Core

open ILGPU
open ILGPU.Runtime
open System
open System.Runtime.CompilerServices

module private AccConstants =

    [<Literal>]
    let deadCell = 0<CellStatus>

    [<Literal>]
    let livingCell = 1<CellStatus>

module private GPUUtils =
    let golKernel
        (index: Index2D)
        (activeBuffer: ArrayView2D<int<CellStatus>, Stride2D.DenseX>)
        (passiveBuffer: ArrayView2D<int<CellStatus>, Stride2D.DenseX>)
        =
        if
            index.X <= 0
            || index.Y <= 0
            || index.X >= int activeBuffer.Extent.X - 1
            || index.Y >= int activeBuffer.Extent.Y - 1
        then
            ()
        else
            let upLeft = new Index2D(index.X - 1, index.Y - 1)
            let up = new Index2D(index.X - 1, index.Y)
            let upRight = new Index2D(index.X - 1, index.Y + 1)
            let left = new Index2D(index.X, index.Y - 1)
            let right = new Index2D(index.X, index.Y + 1)
            let downLeft = new Index2D(index.X + 1, index.Y - 1)
            let down = new Index2D(index.X + 1, index.Y)
            let downRight = new Index2D(index.X + 1, index.Y + 1)

            let sum =
                activeBuffer.[upLeft]
                + activeBuffer.[up]
                + activeBuffer.[upRight]
                + activeBuffer.[left]
                + activeBuffer.[right]
                + activeBuffer.[downLeft]
                + activeBuffer.[down]
                + +activeBuffer.[downRight]

            if sum = 2<CellStatus> then
                passiveBuffer.[index] <- activeBuffer.[index]
            elif sum = 3<CellStatus> then
                passiveBuffer.[index] <- 1<CellStatus>
            else
                passiveBuffer.[index] <- 0<CellStatus>

[<Sealed>]
type AcceleratedConwayGrid
    private
    (
        startingGrid: int<CellStatus> array2d,
        gpuContext: Context,
        accelerator: Accelerator,
        kernel:
            Action<Index2D, ArrayView2D<int<CellStatus>, Stride2D.DenseX>, ArrayView2D<int<CellStatus>, Stride2D.DenseX>>
    ) =

    let _context = gpuContext

    let _accelerator = accelerator

    let _kernel = kernel

    let _gpuBuffers =
        let dimensions =
            new LongIndex2D(Array2D.length1 startingGrid, Array2D.length2 startingGrid)

        let buffer1 = accelerator.Allocate2DDenseX<int<CellStatus>>(&dimensions)
        let buffer2 = accelerator.Allocate2DDenseX<int<CellStatus>>(&dimensions)

        buffer1.CopyFromCPU startingGrid
        buffer2.CopyFromCPU startingGrid

        [| buffer1; buffer2 |]

    private new(width: int, height: int) =
        let initArr = Array2D.create (height + 2) (width + 2) 0<CellStatus>
        let context, accelerator, kernel = AcceleratedConwayGrid.initializeGpu ()
        new AcceleratedConwayGrid(initArr, context, accelerator, kernel)

    member val private Buffers = [| Array2D.copy startingGrid; Array2D.copy startingGrid |] with get, set

    member val private ActiveBufferIndex = 0 with get, set

    member this.Board = this.Buffers[this.ActiveBufferIndex]

    member this.ActiveWidth = Array2D.length2 this.Board - 2

    member this.ActiveHeight = Array2D.length1 this.Board - 2

    static member private initializeGpu() =
        let context = Context.CreateDefault()

        let device =
            context.Devices
            |> Seq.tryFind (fun d -> d.AcceleratorType = AcceleratorType.Cuda)
            |> Option.defaultWith (fun _ ->
                context.Devices
                |> Seq.tryFind (fun d -> d.AcceleratorType = AcceleratorType.OpenCL)
                |> Option.defaultValue (context.Devices |> Seq.exactlyOne))

        let accelerator = device.CreateAccelerator context
        let kernel = accelerator.LoadAutoGroupedStreamKernel<_, _, _> GPUUtils.golKernel

        context, accelerator, kernel

    [<MethodImpl(MethodImplOptions.AggressiveOptimization)>]
    member this.AdvanceToNextState() =
        let activeIndex = this.ActiveBufferIndex
        let passiveIndex = (activeIndex + 1) % this.Buffers.Length

        let activeBuffer = this.Buffers[activeIndex]
        let passiveBuffer = this.Buffers[passiveIndex]

        let rows = Array2D.length1 activeBuffer
        let cols = Array2D.length2 activeBuffer

        _gpuBuffers.[activeIndex].CopyFromCPU activeBuffer

        kernel.Invoke(Index2D(rows, cols), _gpuBuffers.[activeIndex].View, _gpuBuffers.[passiveIndex].View)

        _gpuBuffers.[passiveIndex].CopyToCPU passiveBuffer

        this.ActiveBufferIndex <- passiveIndex

    [<CompiledName("CreateDead")>]
    static member createDead width height =
        new AcceleratedConwayGrid(width, height)

    [<CompiledName("CreateLiving")>]
    static member createLiving width height =
        let context, accelerator, kernel = AcceleratedConwayGrid.initializeGpu ()

        let initArr =
            Array2D.init (height + 2) (width + 2) (fun i j ->
                if i = 0 || j = 0 || i = height + 1 || j = width + 1 then
                    AccConstants.deadCell
                else
                    AccConstants.livingCell)

        new AcceleratedConwayGrid(initArr, context, accelerator, kernel)

    [<CompiledName("CreateRandomWithOdds")>]
    static member createRandomWithOdds width height oddsOfLiving =
        let context, accelerator, kernel = AcceleratedConwayGrid.initializeGpu ()
        let random = new Random()

        let initArr =
            Array2D.init (height + 2) (width + 2) (fun i j ->
                if i = 0 || j = 0 || i = height + 1 || j = width + 1 then
                    AccConstants.deadCell
                else
                    let randomValue = random.Next(0, oddsOfLiving - 1)

                    if randomValue = 0 then
                        AccConstants.livingCell
                    else
                        AccConstants.deadCell)

        new AcceleratedConwayGrid(initArr, context, accelerator, kernel)

    [<CompiledName("Init")>]
    static member init width height initializer =
        let context, accelerator, kernel = AcceleratedConwayGrid.initializeGpu ()

        let initArr =
            Array2D.init (height + 2) (width + 2) (fun i j ->
                if i = 0 || j = 0 || i = height + 1 || j = width + 1 then
                    AccConstants.deadCell
                else
                    initializer (i - 1) (j - 1))

        new AcceleratedConwayGrid(initArr, context, accelerator, kernel)

    [<CompiledName("CopyFrom")>]
    static member copyFrom(otherGrid: AcceleratedConwayGrid) =
        let context, accelerator, kernel = AcceleratedConwayGrid.initializeGpu ()

        new AcceleratedConwayGrid(Array2D.copy otherGrid.Board, context, accelerator, kernel)

    interface IDisposable with
        member _.Dispose() =
            _gpuBuffers |> Array.iter (fun buffer -> buffer.Dispose())

            _accelerator.Dispose()
            _context.Dispose()
