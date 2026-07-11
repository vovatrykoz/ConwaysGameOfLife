namespace Conway.App.Utils

module Conversion =
    open System
    open Raylib_cs

    let inline CBoolToFsBool (cbool: CBool) = Convert.ToBoolean(sbyte cbool)

module Alias =
    let inline raylibTrue expr = expr |> Conversion.CBoolToFsBool
