namespace Conway.App

open Conway.App.Math

module Config =
    module Default =
        let windowWidth = 1024<px>

        let windowHeight = 768<px>

        let gridWidthIndex = 1
        let gridHeightIndex = 2
        let gridWidth = 1000<cells>
        let gridHeight = 1000<cells>

        let canvasX = 25.0f<px>
        let canvasY = 25.0f<px>
        let cellSize = 25.0f<px>

        let widthOffset: float32<px> = cellSize * 12.0f
        let heightOffset: float32<px> = cellSize * 2.0f

        let canvasWidth =
            let windowWidthPx: float32<px> =
                LanguagePrimitives.Float32WithMeasure(float32 windowWidth)

            windowWidthPx - widthOffset

        let canvasHeight =
            let windowHeightPx: float32<px> =
                LanguagePrimitives.Float32WithMeasure(float32 windowHeight)

            windowHeightPx - heightOffset

        let cameraPosX = 500.0f<cells>
        let cameraPosY = 500.0f<cells>

        // 1 in 5 odds that a cell is living
        let oddsOfGettingLivingCell = 5

        let sleepTimeCalculator (gridWidth: int<cells>) (gridHeight: int<cells>) =
            if gridHeight <= 2000<cells> || gridWidth <= 2000<cells> then
                34
            else if gridHeight <= 5000<cells> || gridWidth <= 5000<cells> then
                16
            else if gridHeight <= 8000<cells> || gridWidth <= 8000<cells> then
                8
            else
                4

        let maxFpsSamples = 60
