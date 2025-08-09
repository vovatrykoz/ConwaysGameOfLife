namespace Conway.App

open Raylib_cs

type ParsingStep =
    | ReadingSwitches
    | ReadingWidth
    | ReadingHeight

type ParsingError =
    | UnknownSwitch of string
    | NoWidthProvided
    | NoHeightProvided

type IntCastingError =
    | NullInput
    | InvalidNumber of string
    | NumberTooLarge of string
    | NegativeNumber of int

type UserDefinedValues = {
    WidthResult: Result<int, IntCastingError> option
    HeightResult: Result<int, IntCastingError> option
} with

    static member create widthOption heigthOption = {
        WidthResult = widthOption
        HeightResult = heigthOption
    }

    static member empty = {
        WidthResult = None
        HeightResult = None
    }

module UserInput =

    let widthShortSwitch, widthLongSwitch = "-w", "--width"

    let heightShortSwitch, heightLongSwitch = "-h", "--height"

    let tryReadInt (stringValue: string) =
        try
            match int stringValue with
            | x when x <= 0 -> Error(NegativeNumber x)
            | x -> Ok x
        with
        | :? System.OverflowException -> Error(NumberTooLarge stringValue)
        | :? System.FormatException -> Error(InvalidNumber stringValue)
        | :? System.ArgumentNullException -> Error NullInput

    [<TailCall>]
    let rec private tryReadArgsRec (args: string array) index parsingStep state =
        if index >= args.Length then
            match parsingStep with
            | ReadingWidth -> Error NoWidthProvided
            | ReadingHeight -> Error NoHeightProvided
            | _ -> Ok state
        else
            match parsingStep with
            | ReadingSwitches ->
                match args[index] with
                | "-w"
                | "--width" -> tryReadArgsRec args (index + 1) ReadingWidth state
                | "-h"
                | "--height" -> tryReadArgsRec args (index + 1) ReadingHeight state
                | _ -> Error(UnknownSwitch args[index])
            | ReadingWidth ->
                let conversionResult = tryReadInt args[index]

                match conversionResult with
                | Ok result ->
                    tryReadArgsRec args (index + 1) ReadingSwitches {
                        state with
                            WidthResult = Some(Ok result)
                    }
                | Error err ->
                    tryReadArgsRec args (index + 1) ReadingSwitches {
                        state with
                            WidthResult = Some(Error err)
                    }
            | ReadingHeight ->
                let conversionResult = tryReadInt args[index]

                match conversionResult with
                | Ok result ->
                    tryReadArgsRec args (index + 1) ReadingSwitches {
                        state with
                            HeightResult = Some(Ok result)
                    }
                | Error err ->
                    tryReadArgsRec args (index + 1) ReadingSwitches {
                        state with
                            HeightResult = Some(Error err)
                    }

    let tryReadArgs (args: string array) =
        tryReadArgsRec args 1 ReadingSwitches UserDefinedValues.empty
