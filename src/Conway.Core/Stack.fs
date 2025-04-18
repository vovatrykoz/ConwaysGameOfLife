namespace Conway.Core

type Stack<'T> = private {
    Contents: list<'T>
} with

    [<CompiledName("Empty")>]
    static member empty: Stack<'T> = { Contents = List.empty }

    [<CompiledName("IsEmpty")>]
    static member isEmpty(stack: Stack<'T>) =
        match stack.Contents with
        | [] -> true
        | _ -> false

    [<CompiledName("Push")>]
    static member push (value: 'T) (stack: Stack<'T>) = {
        stack with
            Contents = value :: stack.Contents
    }

    [<CompiledName("Pop")>]
    static member pop(stack: Stack<'T>) =
        match stack.Contents with
        | x :: xs -> x, { stack with Contents = xs }
        | [] -> invalidOp "The stack was empty"

    [<CompiledName("TryPop")>]
    static member tryPop(stack: Stack<'T>) =
        match stack.Contents with
        | x :: xs -> Some x, { stack with Contents = xs }
        | [] -> None, stack

    [<CompiledName("Peek")>]
    static member peek(stack: Stack<'T>) =
        match stack.Contents with
        | x :: _ -> x
        | [] -> invalidOp "The stack was empty"

    [<CompiledName("TryPeek")>]
    static member tryPeek(stack: Stack<'T>) =
        match stack.Contents with
        | x :: _ -> Some x
        | [] -> None
