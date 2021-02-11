namespace FSharp.JsonCompiler

open Xunit
open Xunit.Abstractions
open System
open FSharp.Literals
open FSharp.xUnit
open System.Collections.Generic

type DBNullConverterTest(output: ITestOutputHelper) =

    [<Fact>]
    member this.``read DBNull``() =
        let x = box DBNull.Value
        let y = FSharpReaders.mainRead [|DBNullConverter.DBNullReader|] typeof<DBNull> x
        //output.WriteLine(Render.stringify y)
        Should.equal y <| Json.Null

    [<Fact>]
    member this.``DBNull instantiate``() =
        let x = Json.Null
        let y = 
            FSharpWriters.mainWrite [|DBNullConverter.DBNullWriter|] typeof<DBNull> x
            :?> DBNull

        //output.WriteLine(Render.stringify y)
        Should.equal y <| DBNull.Value

