namespace FSharp.JsonCompiler

open Xunit
open Xunit.Abstractions
open System
open FSharp.Literals
open FSharp.xUnit
open System.Collections.Generic

type DateTimeOffsetConverterTest(output: ITestOutputHelper) =

    [<Fact>]
    member this.``read DateTimeOffset``() =
        let x = DateTimeOffset(2021,2,11,9,2,18,0,TimeSpan(0,8,0,0,0))
        let y = FSharpReaders.mainRead [|DateTimeOffsetConverter.DateTimeOffsetReader|] typeof<DateTimeOffset> x
        //output.WriteLine(Render.stringify x)
        //output.WriteLine(Render.stringify y)
        Should.equal y <| Json.String "2021/2/11 9:02:18 +08:00"

    [<Fact>]
    member this.``DateTimeOffset instantiate``() =
        let x = Json.String "2021/2/11 9:02:18 +08:00"
        let y = 
            FSharpWriters.mainWrite [|DateTimeOffsetConverter.DateTimeOffsetWriter|] typeof<DateTimeOffset> x
            :?> DateTimeOffset

        //output.WriteLine(Render.stringify y)
        Should.equal y <| DateTimeOffset(2021,2,11,9,2,18,0,TimeSpan(0,8,0,0,0))

