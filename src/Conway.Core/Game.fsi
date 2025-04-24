namespace Conway.Core

type GameMode =
    | Infinite
    | Limited of int
    | Paused

type Game =
    class
        new: initialState: ConwayGrid -> Game

        member State: ConwayGrid with get, set

        member Generation: int with get

        member Run: GameMode -> unit

        member RunOneStep: unit -> unit

        member ClearHistory: unit -> unit
    end
