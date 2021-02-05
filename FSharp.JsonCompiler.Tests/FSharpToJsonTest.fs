namespace FSharp.JsonCompiler

open Xunit
open Xunit.Abstractions
open System
open System.Reflection
open System.IO
open System.Text.RegularExpressions
open FSharp.Literals
open FSharp.xUnit

type FSharpToJsonTest(output: ITestOutputHelper) =
    [<Fact>]
    member this.``covert from sbyte test``() =
        let x = 0y
        let y = FSharpToJson.objectToJson (x.GetType()) x
        Assert.Equal(y, Json.SByte x)

    [<Fact>]
    member this.``covert from byte test``() =
        let x = 0uy
        let y = FSharpToJson.objectToJson (x.GetType()) x
        Assert.Equal(y,Json.Byte x)

    [<Fact>]
    member this.``covert from int16 test``() =
        let x = 0s
        let y = FSharpToJson.objectToJson (x.GetType()) x
        Assert.Equal(y,Json.Int16 x)

    [<Fact>]
    member this.``covert from uint16 test``() =
        let x = 0us
        let y = FSharpToJson.objectToJson (x.GetType()) x
        Assert.Equal(y,Json.UInt16 x)

    [<Fact>]
    member this.``covert from int test``() =
        let x = 0
        let y = FSharpToJson.objectToJson (x.GetType()) x
        Assert.Equal(y,Json.Int32 x)

    [<Fact>]
    member this.``covert from uint32 test``() =
        let x = 0u
        let y = FSharpToJson.objectToJson (x.GetType()) x
        Assert.Equal(y,Json.UInt32 x)

    [<Fact>]
    member this.``covert from int64 test``() =
        let x = 0L
        let y = FSharpToJson.objectToJson (x.GetType()) x
        Assert.Equal(y,Json.Int64 x)

    [<Fact>]
    member this.``covert from uint64 test``() =
        let x = 0UL
        let y = FSharpToJson.objectToJson (x.GetType()) x
        Assert.Equal(y,Json.UInt64 x)

    [<Fact>]
    member this.``covert from nativeint test``() =
        let x = 0n
        let y = FSharpToJson.objectToJson (x.GetType()) x
        Assert.Equal(y, Json.IntPtr x)

    [<Fact>]
    member this.``covert from unativeint test``() =
        let x = 0un
        let y = FSharpToJson.objectToJson (x.GetType()) x
        Assert.Equal(y,Json.UIntPtr x)

    [<Fact>]
    member this.``covert from single test``() =
        let x = 0.1f
        let y = FSharpToJson.objectToJson (x.GetType()) x
        Assert.Equal(y,Json.Single x)

    [<Fact>]
    member this.``covert from decimal test``() =
        let x = 0M
        let y = FSharpToJson.objectToJson (x.GetType()) x
        Assert.Equal(y,Json.Decimal x)

    [<Fact>]
    member this.``covert from bigint test``() =
        let x = 0I
        let y = FSharpToJson.objectToJson (x.GetType()) x
        Assert.Equal(y,Json.BigInteger x)

    [<Fact>]
    member this.``covert from char test``() =
        let x = '\t'
        let y = FSharpToJson.objectToJson typeof<char> x
        Assert.Equal(y, Json.Char x)

    [<Fact>]
    member this.``covert from nullable test``() =
        let x0 = Nullable()
        let y0 = FSharpToJson.objectToJson typeof<Nullable<_>> x0
        Assert.Equal(y0,Json.Null)

        let x = Nullable(3)
        let y = FSharpToJson.objectToJson typeof<Nullable<int>> x
        Assert.Equal(y,Json.Int32 x.Value)

    [<Fact>]
    member this.``covert from array test``() =
        let ls = [|1;2;3|]
        let res = FSharpToJson.objectToJson (ls.GetType()) ls
        Assert.Equal(res,Json.Elements (ls |> List.ofArray |> List.map Json.Int32))

    [<Fact>]
    member this.``covert from list test``() =
        let ls = [1;2;3]
        let res = FSharpToJson.objectToJson typeof<List<int>> ls
        Assert.Equal(res,Json.Elements (ls |> List.map Json.Int32))

    [<Fact>]
    member this.``covert from tuple test``() =
        let ls = (1,"x")
        let res = FSharpToJson.objectToJson (ls.GetType()) ls
        Assert.Equal(res,Json.Elements [Json.Int32 1; Json.String "x"])
        
    [<Fact>]
    member this.``covert from set test``() =
        Assert.Equal(FSharpToJson.objectToJson typeof<Set<_>> Set.empty, Json.Elements [])

        let ls = Set.ofList [1;2;3]
        let res = FSharpToJson.objectToJson typeof<Set<int>> ls
        Assert.Equal(res, Json.Elements (ls |> Set.toList |> List.map Json.Int32))

    [<Fact>]
    member this.``covert from map test``() =
        Assert.Equal(FSharpToJson.objectToJson typeof<Map<_,_>> Map.empty, Json.Elements [])

        let x = Map.ofList ["1",1;"2",2;"3",3]
        let y = FSharpToJson.objectToJson typeof<Map<string,int>> x
        //output.WriteLine(Render.stringify y)
        Assert.Equal(y,Json.Elements (x |> Map.toList |> List.map(fun (k, v) -> Json.Elements[Json.String k; Json.Int32 v])))

    [<Fact>]
    member this.``covert from null test``() =
        let ls = null
        let res = FSharpToJson.objectToJson typeof<_> ls
        Should.equal res Json.Null

    [<Fact>]
    member this.``covert from enum test``() =
        let e = FileMode.Open
        let res = ParenRender.instanceToString 0 typeof<FileMode> e
        output.WriteLine(res)

    [<Fact>]
    member this.``covert from record test``() =
        let x = {| name = "xyz"; ``your age`` = 18 |}
        let res = FSharpToJson.objectToJson (x.GetType()) x
        Should.equal res <| Json.Pairs(set ["name", Json.String "xyz";"your age", Json.Int32 18])

    [<Fact>]
    member this.``covert from some test``() =
        let x = Some 123
        let res = FSharpToJson.objectToJson (x.GetType()) x
        Assert.Equal(res, Json.Int32 123)

        Should.equal 
            (FSharpToJson.objectToJson typeof<Option<_>> None) 
            Json.Null

    [<Fact>]
    member this.``covert from flags enum test``() =
        let flags = BindingFlags.Public ||| BindingFlags.NonPublic

        let res = FSharpToJson.objectToJson typeof<BindingFlags> flags
        //output.WriteLine(Render.stringify res)
        Should.equal res <| Json.String "BindingFlags.Public|||BindingFlags.NonPublic"

    [<Fact>]
    member this.``covert from flags none enum test``() =
        let flags = RegexOptions.None
        let res = FSharpToJson.objectToJson typeof<RegexOptions> flags
        //output.WriteLine(Render.stringify res)
        Should.equal res <| Json.String "RegexOptions.None"

