namespace Conway.Compression.Huffman

open System.Collections.Generic

type HuffmanNode<'T> = {
    Weight: int
    Node: HuffmanNodeType<'T>
} with

    [<CompiledName("Create")>]
    static member create weight node = { Weight = weight; Node = node }

    [<CompiledName("CreateInternal")>]
    static member createInternal weight left right =
        HuffmanNode<_>.create weight (HuffmanNodeType<_>.createInternal left right)

    [<CompiledName("CreateLeaf")>]
    static member createLeaf weight value =
        HuffmanNode<_>.create weight (HuffmanNodeType<_>.createLeaf value)

    [<CompiledName("Merge")>]
    static member merge leftNode rightNode =
        HuffmanNode<_>.createInternal (leftNode.Weight + rightNode.Weight) leftNode.Node rightNode.Node

    member this.MergeWith(other: HuffmanNode<_>) = HuffmanNode<_>.merge this other

    [<CompiledName("BuildTreeFromPriorityQueue")>]
    static member buildTreeFromPriorityQueue(priorityQueue: PriorityQueue<HuffmanNode<'T>, int>) =
        match priorityQueue.Count with
        | x when x = 1 -> priorityQueue.Dequeue()
        | x when x >= 2 ->
            while priorityQueue.Count >= 2 do
                let leftNode = priorityQueue.Dequeue()
                let rightNode = priorityQueue.Dequeue()
                let newNode = leftNode.MergeWith rightNode

                priorityQueue.Enqueue(newNode, newNode.Weight)

            priorityQueue.Dequeue()
        | _ -> invalidArg (nameof priorityQueue) "The priority queue was empty. No symbols to encode"

    [<CompiledName("TryBuildTreeFromPriorityQueue")>]
    static member tryBuildTreeFromPriorityQueue(priorityQueue: PriorityQueue<HuffmanNode<'T>, int>) =
        match priorityQueue.Count with
        | x when x = 1 -> Some(priorityQueue.Dequeue())
        | x when x >= 2 ->
            while priorityQueue.Count >= 2 do
                let leftNode = priorityQueue.Dequeue()
                let rightNode = priorityQueue.Dequeue()
                let newNode = leftNode.MergeWith rightNode

                priorityQueue.Enqueue(newNode, newNode.Weight)

            Some(priorityQueue.Dequeue())
        | _ -> None
