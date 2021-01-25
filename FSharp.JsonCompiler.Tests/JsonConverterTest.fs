namespace FSharp.JsonCompiler

open Xunit
open Xunit.Abstractions
open System
open FSharp.Literals
open FSharp.xUnit


type JsonConverterTest(output: ITestOutputHelper) =
    [<Fact>]
    member this.``Array test``() =
        let json = Json.Elements [Json.Int32 32;Json.Int32 20]
        let y = JsonConverter.convert<int[]> json
        let yy = [|32;20|]

        //output.WriteLine(Render.stringify y)
        Should.equal y yy

    [<Fact>]
    member this.``tuple test``() =
        let json = Json.Elements [Json.Int32 32;Json.String "xx"]
        let y = JsonConverter.convert<int*string> json
        let yy = 32,"xx"

        //output.WriteLine(Render.stringify y)
        Should.equal y yy

    [<Fact>]
    member this.``record test``() =
        let json = Json.Pairs (set ["a",Json.Int32 32;"b",Json.String"xx"])

        let y = JsonConverter.convert<{|a:int;b:string|}> json
        let yy = {|a=32;b="xx"|}

        //output.WriteLine(Render.stringify y)
        Should.equal y yy
