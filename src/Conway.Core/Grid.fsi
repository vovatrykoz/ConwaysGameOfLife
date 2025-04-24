namespace Conway.Core

type ConwayGrid =
    class
        private new: startingGrid: Cell array2d -> ConwayGrid
        private new: width: int * height: int -> ConwayGrid
        member Board: Cell array2d
        member AdvanceToNextState: unit -> unit

        [<CompiledName("CreateDead")>]
        static member createDead: width: int -> height: int -> ConwayGrid

        [<CompiledName("CreateLiving")>]
        static member createLiving: width: int -> height: int -> ConwayGrid

        [<CompiledName("CreateRandomWithOdds")>]
        static member createRandomWithOdds: width: int -> height: int -> oddsOfLiving: int -> ConwayGrid

        [<CompiledName("Init")>]
        static member init: width: int -> height: int -> initializer: (int -> int -> Cell) -> ConwayGrid

        [<CompiledName("InitFromPreset")>]
        static member initFromPreset: preset: (int * int * (int -> int -> Cell)) -> ConwayGrid
    end
