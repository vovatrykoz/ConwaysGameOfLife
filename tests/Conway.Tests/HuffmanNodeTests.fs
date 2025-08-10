namespace Conway.Tests

open Conway.Compression.Huffman
open NUnit.Framework
open FsCheck.NUnit

module ``Huffman Node`` =
    //
    module ``Huffman Node Type Tests`` =
        //
        [<Property>]
        let ``The "Left" component of the internal huffman node is correctly assigned on creation``
            (leftValue: HuffmanNode<char>, rightValue: HuffmanNode<char>)
            =
            let internalHuffmanNode = InternalHuffmanNode<char>.create leftValue rightValue

            Assert.That(internalHuffmanNode.Left, Is.EqualTo leftValue)

        [<Property>]
        let ``The "Right" component of the internal huffman node is correctly assigned on creation``
            (leftValue: HuffmanNode<char>, rightValue: HuffmanNode<char>)
            =
            let internalHuffmanNode = InternalHuffmanNode<char>.create leftValue rightValue

            Assert.That(internalHuffmanNode.Right, Is.EqualTo rightValue)

        [<Property>]
        let ``Can create an internal node with the corresponding static member of HuffmanNodeType``
            (leftValue: HuffmanNode<char>, rightValue: HuffmanNode<char>)
            =
            let actual = HuffmanNode<char>.createInternal leftValue rightValue
            let expected = InternalNode(InternalHuffmanNode<char>.create leftValue rightValue)

            Assert.That(actual, Is.EqualTo expected)

        [<Property>]
        let ``Can create a leaf node with the corresponding static member of HuffmanNodeType`` (value: char) =
            let actual = HuffmanNode<char>.createLeaf value
            let expected = Leaf value

            Assert.That(actual, Is.EqualTo expected)

        module ``Huffman Node Tests`` =
            open System
            open System.Collections.Generic
            //
            [<Property>]
            let ``Can create a new node using the corresponding static member ``
                (weight: int, node: HuffmanNode<char>)
                =
                let expected = { Weight = weight; Node = node }
                let actual = WeightedHuffmanNode<char>.create weight node

                Assert.That(actual, Is.EqualTo expected)

            [<Property>]
            let ``Can create an internal node using the corresponding static member ``
                (weight: int, leftNode: HuffmanNode<char>, rightNode: HuffmanNode<char>)
                =
                let expected = {
                    Weight = weight
                    Node = InternalNode(InternalHuffmanNode<_>.create leftNode rightNode)
                }

                let actual = WeightedHuffmanNode<char>.createInternal weight leftNode rightNode

                Assert.That(actual, Is.EqualTo expected)

            [<Property>]
            let ``Can create a leaf node using the corresponding static member `` (weight: int, value: char) =
                let expected = { Weight = weight; Node = Leaf value }
                let actual = WeightedHuffmanNode<char>.createLeaf weight value

                Assert.That(actual, Is.EqualTo expected)

            [<Property>]
            let ``Can merge two nodes using the corresponding static member ``
                (leftNode: WeightedHuffmanNode<char>, rightNode: WeightedHuffmanNode<char>)
                =
                let expected = {
                    Weight = leftNode.Weight + rightNode.Weight
                    Node = InternalNode(InternalHuffmanNode<char>.create leftNode.Node rightNode.Node)
                }

                let actual = WeightedHuffmanNode<char>.merge leftNode rightNode

                Assert.That(actual, Is.EqualTo expected)

            [<Property>]
            let ``Can merge two nodes using the corresponding instance member ``
                (leftNode: WeightedHuffmanNode<char>, rightNode: WeightedHuffmanNode<char>)
                =
                let expected = {
                    Weight = leftNode.Weight + rightNode.Weight
                    Node = InternalNode(InternalHuffmanNode<char>.create leftNode.Node rightNode.Node)
                }

                let actual = leftNode.MergeWith rightNode

                Assert.That(actual, Is.EqualTo expected)

            [<Test>]
            let ``Trying to build a huffman tree from an empty priority queue returns "None"`` () =
                let emptyPriorityQueue = new PriorityQueue<WeightedHuffmanNode<char>, int>()

                let expected = None

                let actual =
                    WeightedHuffmanNode<char>.tryBuildTreeFromPriorityQueue emptyPriorityQueue

                Assert.That(actual, Is.EqualTo expected)

            [<Property>]
            let ``Building a huffman tree from a priority queue with a single element results in a huffman tree with just one node``
                (value: char, frequency: int)
                =
                let priorityQueue = new PriorityQueue<WeightedHuffmanNode<char>, int>()
                let node = WeightedHuffmanNode<char>.createLeaf frequency value
                priorityQueue.Enqueue(node, frequency)

                let root = WeightedHuffmanNode<char>.tryBuildTreeFromPriorityQueue priorityQueue
                let expected = Some node.Node

                Assert.That(root, Is.EqualTo expected)

            [<Property>]
            let ``Building a huffman tree from several leaves results in a correct tree``
                (
                    node1: WeightedHuffmanNode<char>,
                    node2: WeightedHuffmanNode<char>,
                    node3: WeightedHuffmanNode<char>,
                    node4: WeightedHuffmanNode<char>
                ) =
                let priorityQueue = new PriorityQueue<WeightedHuffmanNode<char>, int>()

                priorityQueue.Enqueue(node1, node1.Weight)
                priorityQueue.Enqueue(node2, node2.Weight)
                priorityQueue.Enqueue(node3, node3.Weight)
                priorityQueue.Enqueue(node4, node4.Weight)

                while priorityQueue.Count >= 2 do
                    let left = priorityQueue.Dequeue()
                    let right = priorityQueue.Dequeue()
                    let newNode = left.MergeWith right

                    priorityQueue.Enqueue(newNode, newNode.Weight)

                let expected = Some(priorityQueue.Dequeue().Node)

                priorityQueue.Enqueue(node1, node1.Weight)
                priorityQueue.Enqueue(node2, node2.Weight)
                priorityQueue.Enqueue(node3, node3.Weight)
                priorityQueue.Enqueue(node4, node4.Weight)

                let actual = WeightedHuffmanNode<char>.tryBuildTreeFromPriorityQueue priorityQueue

                Assert.That(actual, Is.EqualTo expected)
