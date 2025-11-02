namespace Conway.App

open Conway.App.Math

module Config =
    module Default =
        let windowWidth = 1024<px>

        let windowHeight = 768<px>

        let gridWidthIndex = 1
        let gridHeightIndex = 2
        let gridWidth = 1000
        let gridHeight = 1000

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

        let sleepTimeCalculator (gridWidth: int) (gridHeight: int) =
            if gridHeight <= 2000 || gridWidth <= 2000 then 34
            else if gridHeight <= 5000 || gridWidth <= 5000 then 16
            else if gridHeight <= 8000 || gridWidth <= 8000 then 8
            else 4

        let maxFpsSamples = 60
