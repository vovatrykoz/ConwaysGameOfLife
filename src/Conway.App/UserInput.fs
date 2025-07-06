namespace Conway.App

open Raylib_cs

module UserInput =
    let tryReadInt (stringValue: string) (parseName: string) (fallbackValue: int) =
        try
            let result =
                match int stringValue with
                | x when x <= 0 ->
                    Raylib.TraceLog(
                        TraceLogLevel.Error,
                        $"The {parseName} cannot be less than or equal to zero. It has to be at least 1"
                    )

                    Raylib.TraceLog(TraceLogLevel.Info, $"Setting {parseName} = {fallbackValue}")
                    fallbackValue
                | x ->
                    Raylib.TraceLog(TraceLogLevel.Info, $"Setting {parseName} = {x}")
                    x

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
