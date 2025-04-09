namespace Conway.App.Raylib

open Raylib_cs

module Convert =
    let CBoolToFsBool (cbool: CBool) =
        match sbyte cbool with
        | 0y -> false
        | _ -> true
