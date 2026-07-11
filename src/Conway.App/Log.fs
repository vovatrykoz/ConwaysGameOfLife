namespace Conway.App

module Log =
    open Raylib_cs
    open System

    let userInputError =
        function
        | UnknownSwitch sw -> Raylib.TraceLog(TraceLogLevel.Error, $"Unknown switch {sw}. Terminating the program")
        | NoWidthProvided ->
            Raylib.TraceLog(
                TraceLogLevel.Error,
                "No width value provided after the width switch (-w|--width) was used. Terminating the program"
            )
        | NoHeightProvided ->
            Raylib.TraceLog(
                TraceLogLevel.Error,
                "No height value provided after the height switch (-h|--height) was used. Terminating the program"
            )

    let dimensionError name =
        function
        | NumberTooLarge value ->
            Raylib.TraceLog(
                TraceLogLevel.Error,
                $"The {name} value was outside of the allowed range: {value}\nLargest allowed value: {Int32.MaxValue}\nSmallest allowed value: 1"
            )
        | InvalidNumber value ->
            Raylib.TraceLog(TraceLogLevel.Error, $"The provided {name} value was not a valid number: {value}")
        | NegativeNumber value ->
            Raylib.TraceLog(
                TraceLogLevel.Error,
                $"The provided {name} value was negative: {value}. Only positive values are allowed"
            )
        | ZeroNumber ->
            Raylib.TraceLog(
                TraceLogLevel.Error,
                $"The provided {name} value was zero. Only positive values are allowed"
            )
        | NullInput -> Raylib.TraceLog(TraceLogLevel.Error, $"The provided {name} string was null")
