namespace Conway.Compression.Huffman

open System.Collections.Generic
open Conway.Encoding

type WeightedHuffmanNode<'T> when 'T: equality = {
    Weight: int
    Node: HuffmanNode<'T>
} with

    [<CompiledName("Create")>]
    static member create weight node = { Weight = weight; Node = node }

    [<CompiledName("CreateInternal")>]
    static member createInternal weight left right =
        WeightedHuffmanNode<_>.create weight (HuffmanNode<_>.createInternal left right)

    [<CompiledName("CreateLeaf")>]
    static member createLeaf weight value =
        WeightedHuffmanNode<_>.create weight (HuffmanNode<_>.createLeaf value)

    [<CompiledName("Merge")>]
    static member merge leftNode rightNode =
        WeightedHuffmanNode<_>.createInternal (leftNode.Weight + rightNode.Weight) leftNode.Node rightNode.Node

    member this.MergeWith(other: WeightedHuffmanNode<_>) = WeightedHuffmanNode<_>.merge this other

    [<CompiledName("BuildTreeFromPriorityQueue")>]
    static member buildTreeFromPriorityQueue(priorityQueue: PriorityQueue<WeightedHuffmanNode<'T>, int>) =
        match priorityQueue.Count with
        | x when x = 1 -> priorityQueue.Dequeue().Node
        | x when x >= 2 ->
            while priorityQueue.Count >= 2 do
                let leftNode = priorityQueue.Dequeue()
                let rightNode = priorityQueue.Dequeue()
                let newNode = leftNode.MergeWith rightNode

                priorityQueue.Enqueue(newNode, newNode.Weight)

            priorityQueue.Dequeue().Node
        | _ -> invalidArg (nameof priorityQueue) "The priority queue was empty. No symbols to encode"

    [<CompiledName("TryBuildTreeFromPriorityQueue")>]
    static member tryBuildTreeFromPriorityQueue(priorityQueue: PriorityQueue<WeightedHuffmanNode<'T>, int>) =
        match priorityQueue.Count with
        | x when x = 1 -> Some(priorityQueue.Dequeue().Node)
        | x when x >= 2 ->
            while priorityQueue.Count >= 2 do
                let leftNode = priorityQueue.Dequeue()
                let rightNode = priorityQueue.Dequeue()
                let newNode = leftNode.MergeWith rightNode

                priorityQueue.Enqueue(newNode, newNode.Weight)

            Some(priorityQueue.Dequeue().Node)
        | _ -> None
