namespace Conway.Compression

open System

[<Interface>]
type IByteDecompressor =
    abstract member Decompress: bytes: ReadOnlySpan<byte> -> byte array
