namespace FSharp.JsonCompiler

open Xunit
open Xunit.Abstractions
open System
open FSharp.Literals
open FSharp.xUnit

type MapTest(output: ITestOutputHelper) =

    [<Fact>]
    member this.``read``() =
        let x = Map.ofList [1,"1";2,"2"]
        let y = FSharpReaders.mainRead [|MapConverter.MapReader|] typeof<Map<int,string>> x
        //output.WriteLine(Render.stringify y)
        Should.equal y 
        <| Json.Elements [Json.Elements [Json.Int32 1;Json.String "1"];Json.Elements [Json.Int32 2;Json.String "2"]]

    [<Fact>]
    member this.``instantiate``() =
        let x = Json.Elements [Json.Elements [Json.Int32 1;Json.String "1"];Json.Elements [Json.Int32 2;Json.String "2"]]
        let y = 
            FSharpWriters.mainWrite [|MapConverter.MapWriter|] typeof<Map<int,string>> x
            :?> Map<int,string>

        //output.WriteLine(Render.stringify y)
        Should.equal y (Map.ofList [1,"1";2,"2"])
