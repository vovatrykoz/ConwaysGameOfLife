namespace Conway.App.Utils

open Raylib_cs
open System

module Convert =
    let inline CBoolToFsBool (cbool: CBool) = Convert.ToBoolean(sbyte cbool)

module Alias =
    let inline raylibTrue expr = expr |> Convert.CBoolToFsBool
