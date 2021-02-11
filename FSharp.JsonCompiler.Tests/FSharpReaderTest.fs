namespace FSharp.JsonCompiler

open Xunit
open Xunit.Abstractions
open System
open System.Reflection
open System.IO
open System.Text.RegularExpressions
open FSharp.Literals
open FSharp.xUnit

type FSharpReaderTest(output: ITestOutputHelper) =
    let readers = [||]
    let read ty obj = FSharpReaders.mainRead readers ty obj

    [<Fact>]
    member this.``covert from sbyte test``() =
        let x = 0y
        let y = read (x.GetType()) x
        Assert.Equal(y, Json.SByte x)

    [<Fact>]
    member this.``covert from byte test``() =
        let x = 0uy
        let y = read (x.GetType()) x
        Assert.Equal(y,Json.Byte x)

    [<Fact>]
    member this.``covert from int16 test``() =
        let x = 0s
        let y = read (x.GetType()) x
        Assert.Equal(y,Json.Int16 x)

    [<Fact>]
    member this.``covert from uint16 test``() =
        let x = 0us
        let y = read (x.GetType()) x
        Assert.Equal(y,Json.UInt16 x)

    [<Fact>]
    member this.``covert from int test``() =
        let x = 0
        let y = read (x.GetType()) x
        Assert.Equal(y,Json.Int32 x)

    [<Fact>]
    member this.``covert from uint32 test``() =
        let x = 0u
        let y = read (x.GetType()) x
        Assert.Equal(y,Json.UInt32 x)

    [<Fact>]
    member this.``covert from int64 test``() =
        let x = 0L
        let y = read (x.GetType()) x
        Assert.Equal(y,Json.Int64 x)

    [<Fact>]
    member this.``covert from uint64 test``() =
        let x = 0UL
        let y = read (x.GetType()) x
        Assert.Equal(y,Json.UInt64 x)

    [<Fact>]
    member this.``covert from nativeint test``() =
        let x = 0n
        let y = read (x.GetType()) x
        Assert.Equal(y, Json.IntPtr x)

    [<Fact>]
    member this.``covert from unativeint test``() =
        let x = 0un
        let y = read (x.GetType()) x
        Assert.Equal(y,Json.UIntPtr x)

    [<Fact>]
    member this.``covert from single test``() =
        let x = 0.1f
        let y = read (x.GetType()) x
        Assert.Equal(y,Json.Single x)

    [<Fact>]
    member this.``covert from decimal test``() =
        let x = 0M
        let y = read (x.GetType()) x
        Assert.Equal(y,Json.Decimal x)

    [<Fact>]
    member this.``covert from bigint test``() =
        let x = 0I
        let y = read (x.GetType()) x
        Assert.Equal(y,Json.BigInteger x)

    [<Fact>]
    member this.``covert from char test``() =
        let x = '\t'
        let y = read typeof<char> x
        Assert.Equal(y, Json.Char x)

    [<Fact>]
    member this.``covert from nullable test``() =
        let x0 = Nullable()
        let y0 = read typeof<Nullable<_>> x0
        Assert.Equal(y0,Json.Null)

        let x = Nullable(3)
        let y = read typeof<Nullable<int>> x
        Assert.Equal(y,Json.Int32 x.Value)

    [<Fact>]
    member this.``covert from null test``() =
        let ls = null
        let res = read typeof<_> ls
        Should.equal res Json.Null

