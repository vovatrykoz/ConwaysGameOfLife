namespace Conway.Tests

open Conway.Core
open NUnit.Framework
open FsCheck
open FsCheck.NUnit

[<Properties(Arbitrary = [| typeof<ConwayGen> |])>]
module ``User Input Processing Tests`` =
    open System.Text.RegularExpressions
    open Conway.App
    open Conway.App.Math
    //
    [<Property>]
    let ``Can parse valid user input`` (input: ValidUserInput) =
        let m =
            Regex.Matches(input.Get, @"(--width|-w|\bwidth\b)\s+(\d+)|(--height|-h|\bheight\b)\s+(\d+)")

        let mutable expectedWidth = 0<cells>
        let mutable expectedHeight = 0<cells>

        for matchObj in m do
            if matchObj.Groups[1].Success then
                expectedWidth <- int matchObj.Groups[2].Value |> LanguagePrimitives.Int32WithMeasure
            elif matchObj.Groups[3].Success then
                expectedHeight <- int matchObj.Groups[4].Value |> LanguagePrimitives.Int32WithMeasure

        let args =
            input.Get.Split([| ' ' |], System.StringSplitOptions.RemoveEmptyEntries)
            |> Array.append [| "exe" |]

        let actual = UserInput.tryReadArgs args

        let expected =
            Result<UserDefinedValues, ParsingError>
                .Ok(
                    UserDefinedValues.create
                        (Some(Result<int<cells>, IntCastingError>.Ok expectedWidth))
                        (Some(Result<int<cells>, IntCastingError>.Ok expectedHeight))
                )

        Assert.That(actual, Is.EqualTo expected)
