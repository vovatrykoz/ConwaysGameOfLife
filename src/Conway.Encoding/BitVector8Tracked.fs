namespace Conway.Encoding

[<Struct>]
type BitVector8Tracked private (b: byte, usedBits: byte) =
    member private _.Byte = b

    member _.UsedBits = usedBits

    [<CompiledName("Zeroed")>]
    static member zeroed = BitVector8Tracked(0b0000_0000uy, 0uy)

    [<CompiledName("AllOnes")>]
    static member allOnes = BitVector8Tracked(0b1111_1111uy, 8uy)

    member bv.ReadBitAt(position: byte) =
        if position >= bv.UsedBits || position < 0uy then
            invalidArg (nameof position) "The position was outside of the allowed range"
        else
            bv.Byte >>> int32 position &&& 1uy = 1uy

    [<CompiledName("ReadBit")>]
    static member inline readBit (position: byte) (bitVector: BitVector8Tracked) = bitVector.ReadBitAt position

    [<CompiledName("PushBit")>]
    static member pushBit (value: bool) (bitVector: BitVector8Tracked) =
        if bitVector.UsedBits >= 8uy then
            bitVector
        else
            match value with
            | true -> BitVector8Tracked(bitVector.Byte ||| (1uy <<< int32 bitVector.UsedBits), bitVector.UsedBits + 1uy)
            | false -> BitVector8Tracked(bitVector.Byte, bitVector.UsedBits + 1uy)

    [<CompiledName("PopBit")>]
    static member popBit(bitVector: BitVector8Tracked) =
        if bitVector.UsedBits <= 0uy then
            bitVector
        else
            BitVector8Tracked(bitVector.Byte &&& ~~~(1uy <<< int32 bitVector.UsedBits), bitVector.UsedBits - 1uy)
