namespace Conway.App.Raylib

open Raylib_cs
open System

// Warning FS0042: This construct is deprecated: it is only for use in the F# library
#nowarn "42"

module private UnsafeUtils =

    // This is generally bad and should be avoided
    // As seen from the warning, should only really be used in the core F# library
    // This IL instruction essentially does a "reinterpret_cast", except with values instead of pointers
    // This idea was taken from the "F# for Performance-Critical Code" talk by Matthew Crews
    let inline retype<'T, 'U> (x: 'T) : 'U = (# "" x: 'U #)

module Convert =
    let inline CBoolToFsBool (cbool: CBool) = Convert.ToBoolean(sbyte cbool)

module Aliases =
    let inline raylibTrue expr = expr |> Convert.CBoolToFsBool
