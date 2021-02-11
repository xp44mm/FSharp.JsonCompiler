namespace FSharp.JsonCompiler

open Xunit
open Xunit.Abstractions
open System
open FSharp.Literals
open FSharp.xUnit

type SetTest(output: ITestOutputHelper) =

    [<Fact>]
    member this.``read set``() =
        let x = set [1;2;3]
        let y = FSharpReaders.mainRead [|SetConverter.SetReader|] typeof<Set<int>> x
        //output.WriteLine(Render.stringify y)
        Should.equal y <| Json.Elements [Json.Int32 1;Json.Int32 2;Json.Int32 3]

    [<Fact>]
    member this.``set instantiate``() =
        let x = Json.Elements [Json.Int32 1;Json.Int32 2;Json.Int32 3]
        let y = 
            FSharpWriters.mainWrite [|SetConverter.SetWriter|] typeof<Set<int>> x
            :?> Set<int>

        //output.WriteLine(Render.stringify y)
        Should.equal y (set[1;2;3])

