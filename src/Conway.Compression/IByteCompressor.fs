namespace Conway.Compression

open System

[<Interface>]
type IByteCompressor =
    abstract member Compress: bytes: ReadOnlySpan<byte> -> byte array
