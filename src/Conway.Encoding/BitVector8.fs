namespace Conway.Encoding

[<Struct>]
type BitVector8 private (b: byte) =
    member _.Byte = b

    [<CompiledName("Zeroed")>]
    static member zeroed = BitVector8 0b0000_0000uy

    [<CompiledName("AllOnes")>]
    static member allOnes = BitVector8 0b1111_1111uy

    [<CompiledName("CreateFromByte")>]
    static member createFromByte b = BitVector8 b

    member inline bv.ReadBitAt(position: int) = bv.Byte >>> position &&& 1uy = 1uy

    member bv.SetBitAt(position: int) =
        BitVector8(bv.Byte ||| (1uy <<< position))

    member bv.ClearBitAt(position: int) =
        BitVector8(bv.Byte &&& ~~~(1uy <<< position))
