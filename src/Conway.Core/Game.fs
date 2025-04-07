namespace Conway.Core

type GameMode =
    | Infinite
    | Limited of int

type Game =
    static member run mode startingGrid =
        let mutable grid = startingGrid

        match mode with
        | Infinite ->
            while true do
                grid <- Grid.next grid
        | Limited steps ->
            for _ = 1 to steps do
                grid <- Grid.next grid
