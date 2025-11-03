namespace Conway.App.Utils

open Raylib_cs
open System

// Warning FS0042: This construct is deprecated: it is only for use in the F# library
#nowarn "42"

[<RequireQualifiedAccess>]
module private UnsafeUtils =

    // This is generally bad and should be avoided
    // As seen from the warning, should only really be used in the core F# library
    // This IL instruction essentially does a "reinterpret_cast", except with values instead of pointers
    // This idea was taken from the "F# for Performance-Critical Code" talk by Matthew Crews
    let inline retype<'T, 'U> (x: 'T) : 'U = (# "" x: 'U #)

    let inline compare (ptr1: voidptr) (ptr2: voidptr) : bool = (# "ceq" ptr1 ptr2 : bool #)

module Convert =
    let inline CBoolToFsBool (cbool: CBool) = Convert.ToBoolean(sbyte cbool)

module Alias =
    let inline raylibTrue expr = expr |> Convert.CBoolToFsBool
