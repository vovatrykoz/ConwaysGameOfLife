namespace Conway.Tests

open Conway.App.Controls
open NUnit.Framework

module ``File Picker Tests`` =
    open System

    [<Test>]
    let ``Can correctly calculate visible start index`` () =
        let randomFiles = [
            FileData.createRecord "File1" "./File1" DateTime.Now
            FileData.createRecord "File2" "./File2" DateTime.Now
            FileData.createRecord "File3" "./File3" DateTime.Now
            FileData.createRecord "File4" "./File4" DateTime.Now
            FileData.createRecord "File5" "./File5" DateTime.Now
        ]

        let filePicker = new FilePicker(0.0f, 0.0f, 8.0f, 4.0f, 8.0f, 2.0f, randomFiles)
        filePicker.Camera.MoveCameraDown 3.0f

        let expectedStartIndex = 1
        let struct (actualStartIndex, _) = filePicker.CalculateVisibleIndexRange()

        Assert.That(actualStartIndex, Is.EqualTo expectedStartIndex)

    [<Test>]
    let ``Can correctly calculate visible end index`` () =
        let randomFiles = [
            FileData.createRecord "File1" "./File1" DateTime.Now
            FileData.createRecord "File2" "./File2" DateTime.Now
            FileData.createRecord "File3" "./File3" DateTime.Now
            FileData.createRecord "File4" "./File4" DateTime.Now
            FileData.createRecord "File5" "./File5" DateTime.Now
        ]

        let filePicker = new FilePicker(0.0f, 0.0f, 8.0f, 4.0f, 8.0f, 2.0f, randomFiles)
        filePicker.Camera.MoveCameraDown 3.0f

        let expectedEndIndex = 3
        let struct (_, actualEndIndex) = filePicker.CalculateVisibleIndexRange()

        Assert.That(actualEndIndex, Is.EqualTo expectedEndIndex)
