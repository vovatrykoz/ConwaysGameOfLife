namespace Conway.Tests

open Conway.Core
open NUnit.Framework
open FsCheck
open FsCheck.NUnit

[<Properties(Arbitrary = [| typeof<ConwayGen> |])>]
module ``User Input Processing Tests`` =
    open System.Text.RegularExpressions
    open Conway.App
    //
    [<Property>]
    let ``Can parse valid user input`` (input: ValidUserInput) =
        let m =
            Regex.Matches(input.Get, @"(--width|-w|\bwidth\b)\s+(\d+)|(--height|-h|\bheight\b)\s+(\d+)")

        let mutable expectedWidth = 0
        let mutable expectedHeight = 0

        for matchObj in m do
            if matchObj.Groups[1].Success then
                expectedWidth <- int matchObj.Groups[2].Value
            elif matchObj.Groups[3].Success then
                expectedHeight <- int matchObj.Groups[4].Value

        let args =
            input.Get.Split([| ' ' |], System.StringSplitOptions.RemoveEmptyEntries)
            |> Array.append [| "exe" |]

        let actual = UserInput.tryReadArgs args

        let expected =
            Result<UserDefinedValues, ParsingError>
                .Ok(
                    UserDefinedValues.create
                        (Some(Result<int, IntCastingError>.Ok expectedWidth))
                        (Some(Result<int, IntCastingError>.Ok expectedHeight))
                )

        Assert.That(actual, Is.EqualTo expected)
