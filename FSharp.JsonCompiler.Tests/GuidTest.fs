namespace FSharp.JsonCompiler

open Xunit
open Xunit.Abstractions
open System
open FSharp.Literals
open FSharp.xUnit

type GuidTest(output: ITestOutputHelper) =

    [<Fact>]
    member this.``read``() =
        let x = Guid("936da01f-9abd-4d9d-80c7-02af85c822a8")
        let y = FSharpReaders.mainRead [|GuidConverter.GuidReader|] typeof<Guid> x
        //output.WriteLine(Render.stringify y)
        Should.equal y 
        <| Json.String "936da01f-9abd-4d9d-80c7-02af85c822a8"

    [<Fact>]
    member this.``instantiate``() =
        let x = Json.String "936da01f-9abd-4d9d-80c7-02af85c822a8"
        let y = 
            FSharpWriters.mainWrite [|GuidConverter.GuidWriter|] typeof<Guid> x
            :?> Guid

        //output.WriteLine(Render.stringify y)
        Should.equal y <| Guid("936da01f-9abd-4d9d-80c7-02af85c822a8")

