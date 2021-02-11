namespace FSharp.JsonCompiler

open Xunit
open Xunit.Abstractions
open System
open FSharp.Literals
open FSharp.xUnit

type UionExample =
| Zero
| OnlyOne of int
| Pair of int * string

type UnionTest(output: ITestOutputHelper) =

    [<Fact>]
    member this.``zero union case``() =
        let x = Zero
        let y = FSharpReaders.mainRead [|UnionConverter.UnionReader|] typeof<UionExample> x
        //output.WriteLine(Render.stringify y)
        Should.equal y <| Json.Fields(set ["Zero",Json.Null])

    [<Fact>]
    member this.``zero union case instantiate``() =
        let x = Json.Fields(set ["Zero",Json.Null])
        let y = 
            FSharpWriters.mainWrite [|UnionConverter.UnionWriter|] typeof<UionExample> x
            :?> UionExample

        //output.WriteLine(Render.stringify y)
        Should.equal y Zero

    [<Fact>]
    member this.``only one union case``() =
        let x = OnlyOne 1
        let y = FSharpReaders.mainRead [|UnionConverter.UnionReader|] typeof<UionExample> x
        //output.WriteLine(Render.stringify y)
        Should.equal y <| Json.Fields(set ["OnlyOne",Json.Int32 1])

    [<Fact>]
    member this.``only one union case instantiate``() =
        let x = Json.Fields(set ["OnlyOne",Json.Int32 1])
        let y = 
            FSharpWriters.mainWrite [|UnionConverter.UnionWriter|] typeof<UionExample> x
            :?> UionExample

        //output.WriteLine(Render.stringify y)
        Should.equal y <| OnlyOne 1

    [<Fact>]
    member this.``many params union case``() =
        let x = Pair(1,"")
        let y = FSharpReaders.mainRead [|UnionConverter.UnionReader|] typeof<UionExample> x
        //output.WriteLine(Render.stringify y)
        Should.equal y <| Json.Fields(set ["Pair",Json.Elements [Json.Int32 1;Json.String ""]])

    [<Fact>]
    member this.``many params union case instantiate``() =
        let x = Json.Fields(set ["Pair",Json.Elements [Json.Int32 1;Json.String ""]])
        let y = 
            FSharpWriters.mainWrite [|UnionConverter.UnionWriter|] typeof<UionExample> x
            :?> UionExample

        Should.equal y <| Pair(1,"")

