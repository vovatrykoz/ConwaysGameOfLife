namespace rec Conway.Compression

type HuffmanTree<'T> =
    | Leaf of 'T
    | Node of HuffmanNode<'T>

    static member createLeaf<'T>(value: 'T) = Leaf value

    static member createNode<'T> (left: HuffmanTree<'T>) (right: HuffmanTree<'T>) = HuffmanNode<'T>.create left right

type HuffmanNode<'T> = {
    Left: HuffmanTree<'T>
    Right: HuffmanTree<'T>
} with

    static member create<'T> (left: HuffmanTree<'T>) (right: HuffmanTree<'T>) = { Left = left; Right = right }
