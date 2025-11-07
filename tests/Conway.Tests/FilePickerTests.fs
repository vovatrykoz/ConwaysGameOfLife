namespace Conway.Tests

open Conway.App.Controls
open Conway.App.Math
open NUnit.Framework

module ``File Picker Tests`` =
    open System

    [<Test>]
    let ``Can correctly calculate visible start index`` () =
        let randomFiles = [
            FileData.create "File1" "./File1" Other DateTime.Now
            FileData.create "File2" "./File2" Other DateTime.Now
            FileData.create "File3" "./File3" Other DateTime.Now
            FileData.create "File4" "./File4" Other DateTime.Now
            FileData.create "File5" "./File5" Other DateTime.Now
        ]

        let filePicker =
            new FilePicker(0.0f<px>, 0.0f<px>, 8.0f<px>, 4.0f<px>, 8.0f<px>, 2.0f<px>, randomFiles)

        filePicker.Camera.MoveCameraDown 3.0f<px>

        let expectedStartIndex = 1
        let struct (actualStartIndex, _) = filePicker.CalculateVisibleIndexRange()

        Assert.That(actualStartIndex, Is.EqualTo expectedStartIndex)

    [<Test>]
    let ``Can correctly calculate visible end index`` () =
        let randomFiles = [
            FileData.create "File1" "./File1" Other DateTime.Now
            FileData.create "File2" "./File2" Other DateTime.Now
            FileData.create "File3" "./File3" Other DateTime.Now
            FileData.create "File4" "./File4" Other DateTime.Now
            FileData.create "File5" "./File5" Other DateTime.Now
        ]

        let filePicker =
            new FilePicker(0.0f<px>, 0.0f<px>, 8.0f<px>, 4.0f<px>, 8.0f<px>, 2.0f<px>, randomFiles)

        filePicker.Camera.MoveCameraDown 3.0f<px>

        let expectedEndIndex = 3
        let struct (_, actualEndIndex) = filePicker.CalculateVisibleIndexRange()

        Assert.That(actualEndIndex, Is.EqualTo expectedEndIndex)
