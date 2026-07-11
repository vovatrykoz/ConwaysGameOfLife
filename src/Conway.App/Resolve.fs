namespace Conway.App

module Resolve =
    open Raylib_cs

    let dimension name defaultValue =
        function
        | None ->
            Raylib.TraceLog(TraceLogLevel.Warning, $"No {name} value provided, using default: {defaultValue}")
            Ok defaultValue
        | Some(Ok value) ->
            Raylib.TraceLog(TraceLogLevel.Info, $"Setting {name} = {value}")
            Ok value
        | Some(Error err) ->
            Log.dimensionError name err
            Error()
