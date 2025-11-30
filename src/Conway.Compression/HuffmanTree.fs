namespace rec Conway.Compression

type HuffmanTree<'T> =
    | Leaf of 'T
    | Node of HuffmanNode<'T>

    static member createLeaf<'T> value = Leaf value

    static member createNode<'T> left right = HuffmanNode<_>.create left right

type HuffmanNode<'T> = {
    Left: HuffmanTree<'T>
    Right: HuffmanTree<'T>
} with

    static member create<'T> left right = { Left = left; Right = right }
