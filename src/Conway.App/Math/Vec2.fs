namespace Conway.App.Math

open System.Numerics

[<Struct; StructuralEquality; StructuralComparison>]
type Vec2<[<Measure>] 'u> = {
    mutable X: float32<'u>
    mutable Y: float32<'u>
} with

    static member inline create<'u> (x: float32<'u>) (y: float32<'u>) = { X = x; Y = y }

    static member inline add<'u> (a: Vec2<'u>) (b: Vec2<'u>) = { X = a.X + b.X; Y = a.Y + b.Y }

    static member inline subtract<'u> (a: Vec2<'u>) (b: Vec2<'u>) = { X = a.X - b.X; Y = a.Y - b.Y }

    static member inline multCoordwise<'u> (a: Vec2<'u>) (b: Vec2<1>) = { X = a.X * b.X; Y = a.Y * b.Y }

    static member inline (+)(a: Vec2<'u>, b: Vec2<'u>) = Vec2.add a b

    static member inline (-)(a: Vec2<'u>, b: Vec2<'u>) = Vec2.subtract a b

    static member inline (*)(a: Vec2<'u>, b: Vec2<1>) = Vec2.multCoordwise a b

    static member inline scaleBy<'u> (value: float32) (v: Vec2<'u>) = { X = v.X * value; Y = v.Y * value }

    static member inline fromNumericVector<'u>(v: Vector2) = {
        X = LanguagePrimitives.Float32WithMeasure<'u> v.X
        Y = LanguagePrimitives.Float32WithMeasure<'u> v.Y
    }
