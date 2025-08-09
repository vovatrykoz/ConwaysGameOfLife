namespace Conway.Compression.Huffman

type HuffmanNodeType<'T> =
    | InternalNode of InternalHuffmanNode<'T>
    | Leaf of 'T

    [<CompiledName("CreateInternal")>]
    static member createInternal left right =
        InternalNode(InternalHuffmanNode<_>.create left right)

    [<CompiledName("CreateLeaf")>]
    static member createLeaf value = Leaf value

and InternalHuffmanNode<'T> = {
    Left: HuffmanNodeType<'T>
    Right: HuffmanNodeType<'T>
} with

    [<CompiledName("Create")>]
    static member create left right = { Left = left; Right = right }
