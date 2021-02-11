namespace FSharp.JsonCompiler

open Xunit
open Xunit.Abstractions
open System
open FSharp.Literals
open FSharp.xUnit
open System.Collections.Generic

type TimeSpanConverterTest(output: ITestOutputHelper) =

    [<Fact>]
    member this.``read DateTimeOffset``() =
        let x = TimeSpan(2, 14, 18)
        let y = FSharpReaders.mainRead [|TimeSpanConverter.TimeSpanReader|] typeof<TimeSpan> x

        //output.WriteLine(Render.stringify y)
        Should.equal y <| Json.String "02:14:18"

    [<Fact>]
    member this.``DateTimeOffset instantiate``() =
        let x = Json.String "02:14:18"
        let y = 
            FSharpWriters.mainWrite [|TimeSpanConverter.TimeSpanWriter|] typeof<TimeSpan> x
            :?> TimeSpan

        //output.WriteLine(Render.stringify y)
        Should.equal y <| TimeSpan(2, 14, 18)

