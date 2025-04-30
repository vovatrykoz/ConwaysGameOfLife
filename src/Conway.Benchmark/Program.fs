open Conway.Core
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Diagnosers
open BenchmarkDotNet.Running

[<HardwareCounters(HardwareCounter.BranchMispredictions, HardwareCounter.BranchInstructions, HardwareCounter.CacheMisses)>]
type ConwayGrid() =
    let data = ConwayGrid.createDead 10000 10000

    [<Benchmark>]
    member _.NextLifeSafeNoChuncks() = data.AdvanceToNextState()

    [<Benchmark>]
    member _.NextLifeSafeChunk() = data.AdvanceToNextStateOther()

    [<Benchmark>]
    member _.NextLifeUnsafeNoChuncks() = data.AdvanceToNextStateOtherUnsafe()

    [<Benchmark>]
    member _.NextLifeUnsafeChuncks() =
        data.AdvanceToNextStateOtherUnsafeChunk()

[<EntryPoint>]
let main _ =
    BenchmarkRunner.Run typeof<ConwayGrid> |> ignore
    0
