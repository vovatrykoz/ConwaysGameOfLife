namespace Conway.App

open Raylib_cs

module UserInput =
    let tryReadInt (stringValue: string) (parseName: string) (fallbackValue: int) =
        try
            let result = int stringValue
            Raylib.TraceLog(TraceLogLevel.Info, $"Setting grid height to: {result}")
            result
        with ex ->
            let exceptionString = ex.ToString().Replace("\n", "\n\t")
            Raylib.TraceLog(TraceLogLevel.Error, $"Could not parse the {parseName}. Given: {stringValue}")
            Raylib.TraceLog(TraceLogLevel.Error, $"Detailed error information:\n\t{exceptionString}")
            Raylib.TraceLog(TraceLogLevel.Info, $"Setting the grid to the default height value: {fallbackValue}")

            fallbackValue

    let tryReadArg args index name defaultVal =
        if Array.length args > index then
            tryReadInt args[index] name defaultVal
        else
            Raylib.TraceLog(TraceLogLevel.Info, $"No {name} provided. Using default: {defaultVal}")
            defaultVal
