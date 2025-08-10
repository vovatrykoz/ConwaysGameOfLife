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

    [<CompiledName("ReadBit")>]
    static member inline readBit (position: int) (bitVector: BitVector8) =
        bitVector.Byte >>> position &&& 1uy = 1uy

    member inline bv.ReadBitAt(position: int) = BitVector8.readBit position bv

    [<CompiledName("SetBit")>]
    static member setBit (position: int) (bitVector: BitVector8) =
        BitVector8(bitVector.Byte ||| (1uy <<< position))

    member inline bv.SetBitAt(position: int) = BitVector8.setBit position bv

    [<CompiledName("ClearBit")>]
    static member clearBit (position: int) (bitVector: BitVector8) =
        BitVector8(bitVector.Byte &&& ~~~(1uy <<< position))

    member inline bv.ClearBitAt(position: int) = BitVector8.clearBit position bv
