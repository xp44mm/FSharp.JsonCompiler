namespace FSharp.JsonCompiler

open Xunit
open Xunit.Abstractions
open System
open FSharp.Literals
open FSharp.xUnit
open System.Collections.Generic
open System.Reflection
open System.Text.RegularExpressions

type DataEntry =
| IntegerNumber  = 0
| FloatingNumber = 1
| CharString     = 2
| DateTime       = 3

type EnumTest(output: ITestOutputHelper) =

    [<Fact>]
    member this.``read enum``() =
        let x = DataEntry.DateTime
        let y = FSharpReaders.mainRead [|EnumConverter.EnumReader|] typeof<DataEntry> x

        //output.WriteLine(Render.stringify y)
        Should.equal y <| Json.String "DateTime"

    [<Fact>]
    member this.``enum instantiate``() =
        let x = Json.String "DateTime"
        let y = 
            FSharpWriters.mainWrite [|EnumConverter.EnumWriter|] typeof<DataEntry> x
            :?> DataEntry

        //output.WriteLine(Render.stringify y)
        Should.equal y DataEntry.DateTime
         
    [<Fact>]
    member this.``read flags``() =
        let x = BindingFlags.Public ||| BindingFlags.NonPublic
        let y = FSharpReaders.mainRead [|EnumConverter.EnumReader|] typeof<BindingFlags> x
        //output.WriteLine(Render.stringify res)
        Should.equal y <| Json.Elements [ Json.String "Public"; Json.String "NonPublic" ]

    [<Fact>]
    member this.``flags instantiate``() =
        let x = Json.Elements [ Json.String "Public"; Json.String "NonPublic" ]
        let y = 
            FSharpWriters.mainWrite [|EnumConverter.EnumWriter|] typeof<BindingFlags> x
            :?> BindingFlags

        //output.WriteLine(Render.stringify y)
        Should.equal y (BindingFlags.Public ||| BindingFlags.NonPublic)

    [<Fact>]
    member this.``read zero flags``() =
        let x = RegexOptions.None
        let y = FSharpReaders.mainRead [|EnumConverter.EnumReader|] typeof<RegexOptions> x
        //output.WriteLine(Render.stringify res)
        Should.equal y <| Json.Elements [ Json.String "None"]

    [<Fact>]
    member this.``zero flags instantiate``() =
        let x = Json.Elements [ Json.String "None"]
        let y = 
            FSharpWriters.mainWrite [|EnumConverter.EnumWriter|] typeof<RegexOptions> x
            :?> RegexOptions

        //output.WriteLine(Render.stringify y)
        Should.equal y RegexOptions.None
