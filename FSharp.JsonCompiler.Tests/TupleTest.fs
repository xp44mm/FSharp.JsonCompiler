namespace FSharp.JsonCompiler

open Xunit
open Xunit.Abstractions
open System
open FSharp.Literals
open FSharp.xUnit
open System.Collections.Generic

type TupleTest(output: ITestOutputHelper) =

    [<Fact>]
    member this.``read array``() =
        let x = (1,"x")
        let y = FSharpReaders.mainRead [|TupleConverter.TupleReader|] typeof<int*string> x
        //output.WriteLine(Render.stringify y)
        Should.equal y <| Json.Elements [Json.Int32 1;Json.String "x"]

    [<Fact>]
    member this.``array instantiate``() =
        let x = Json.Elements [Json.Int32 1;Json.String "x"]
        let y = 
            FSharpWriters.mainWrite [|TupleConverter.TupleWriter|] typeof<int*string> x
            :?> int*string

        //output.WriteLine(Render.stringify y)
        Should.equal y (1,"x")
        
