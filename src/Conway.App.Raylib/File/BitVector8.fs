namespace Conway.App.Raylib.File

open System.Runtime.CompilerServices

[<IsReadOnly; Struct>]
type BitVector8 private (b: byte) =
    member _.Byte = b

    static member zeroed = BitVector8 0b0000_0000uy

    static member allOnes = BitVector8 0b1111_1111uy

    static member createFromByte b = BitVector8 b

    member inline bv.ReadBitAt(position: int) = bv.Byte >>> position &&& 1uy = 1uy

    member bv.SetBitAt(position: int) =
        BitVector8(bv.Byte ||| (1uy <<< position))

    member bv.ClearBitAt(position: int) =
        BitVector8(bv.Byte &&& ~~~(1uy <<< position))
