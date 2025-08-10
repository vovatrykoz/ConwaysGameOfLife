namespace Conway.Compression.Huffman

open Conway.Encoding
open System.Collections.Generic

type HuffmanNode<'T> when 'T: equality =
    | Leaf of 'T
    | InternalNode of InternalHuffmanNode<'T>

    [<CompiledName("CreateInternal")>]
    static member createInternal left right =
        InternalNode(InternalHuffmanNode<_>.create left right)

    [<CompiledName("CreateLeaf")>]
    static member createLeaf value = Leaf value

    static member private buildEncodingDictionaryRec
        (treeStack: Stack<HuffmanNode<'T> * bool>)
        (result: Dictionary<'T, BitVector8Tracked>)
        =
        if treeStack.Count = 0 then
            result
        else
            let currentNode, isleft = treeStack.Pop()

            match currentNode with
            | Leaf value ->
                result.Add(value, BitVector8Tracked.zeroed |> BitVector8Tracked.pushBit isleft)
                HuffmanNode<_>.buildEncodingDictionaryRec treeStack result
            | InternalNode node ->
                treeStack.Push((node.Right, false))
                treeStack.Push((node.Left, true))
                HuffmanNode<_>.buildEncodingDictionaryRec treeStack result

    static member buildEncodingDictionary(root: HuffmanNode<'T>) =
        match root with
        | Leaf value ->
            let result = new Dictionary<'T, BitVector8Tracked>()
            result.Add(value, BitVector8Tracked.zeroed |> BitVector8Tracked.pushBit false)
            result
        | InternalNode _ ->
            let treeStack = new Stack<HuffmanNode<'T> * bool>()
            treeStack.Push((root, false))
            HuffmanNode<_>.buildEncodingDictionaryRec treeStack (new Dictionary<'T, BitVector8Tracked>())

and InternalHuffmanNode<'T> when 'T: equality = {
    Left: HuffmanNode<'T>
    Right: HuffmanNode<'T>
} with

    [<CompiledName("Create")>]
    static member create left right = { Left = left; Right = right }
