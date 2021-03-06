﻿namespace FSharp.JsonCompiler

open Xunit
open Xunit.Abstractions
open FSharp.JsonCompiler
open FSharp.Literals
open FSharp.xUnit

type DemoTest(output: ITestOutputHelper) =
    let show outp =
        outp
        |> Render.stringify
        |> output.WriteLine

    [<Fact>]
    member this.``Deserializing test``() =
        let text = """
        {
          "Name": "Apple",
          "ExpiryDate": "2008-12-28T00:00:00",
          "Price": 3.99,
          "Sizes": [
            "Small",
            "Medium",
            "Large"
          ]
        }
        """
        let y = Json.parse text
        show y

        let x = Json.stringify y
        output.WriteLine(x)

        let tree = Json.Fields(set [
            "ExpiryDate",Json.String "2008-12-28T00:00:00";
            "Name",Json.String "Apple";
            "Price",Json.Double 3.99;
            "Sizes",Json.Elements [
                Json.String "Small";
                Json.String "Medium";
                Json.String "Large"]])
        Should.equal y tree

    [<Fact>]
    member this.``Navigate test``() =

        let tree = Json.Fields(set [
            "ExpiryDate",Json.String "2008-12-28T00:00:00";
            "Name",Json.String "Apple";
            "Price",Json.Double 3.99;
            "Sizes",Json.Elements [
                Json.String "Small";
                Json.String "Medium";
                Json.String "Large"]])

        let name = tree.["Name"]
        Should.equal name (Json.String "Apple")

        let size0 = tree.["Sizes"].[0]
        Should.equal size0 (Json.String "Small")

    [<Fact>]
    member this.``htmlColor test``() =
        let tree = Json.Fields(set ["Blue",Json.Int32 0;"Green",Json.Int32 0;"Red",Json.Int32 255])
        let x = JsonRender.stringify tree
        let y = JsonDriver.parse x
        show y
        Should.equal y tree

    [<Fact>]
    member this.``roles test``() =
        let tree = Json.Elements [Json.String "User";Json.String "Admin"]
        let x = JsonRender.stringify tree
        let y = JsonDriver.parse x
        show y
        Should.equal y tree

    [<Fact>]
    member this.``dailyRegistrations test``() =
        let tree = Json.Fields(set ["2014-06-01T00:00:00",Json.Int32 23;"2014-06-02T00:00:00",Json.Int32 50])
        let x = JsonRender.stringify tree
        let y = JsonDriver.parse x
        show y
        Should.equal y tree

    [<Fact>]
    member this.``city test``() =
        let tree = Json.Fields(set ["Name",Json.String "Oslo";"Population",Json.Int32 650000])

        let x = JsonRender.stringify tree
        let y = JsonDriver.parse x
        show y
        Should.equal y tree

    [<Fact>]
    member this.``SerializationBasics2 test``() =
        let tree = Json.Fields(set ["Date",Json.String "new Date(1401796800000)";"Name",Json.String "Serialize All The Things"])

        let x = JsonRender.stringify tree
        let y = JsonDriver.parse x
        show y
        Should.equal y tree

    [<Fact>]
    member this.``SerializeReferencesByValue test``() =
        let tree = Json.Fields(set ["Name",Json.String "Mike Manager";"Reportees",Json.Elements [Json.Fields(set ["Name",Json.String "Arnie Admin"]);Json.Fields(set ["Name",Json.String "Susan Supervisor";"Reportees",Json.Elements [Json.Fields(set ["Name",Json.String "Arnie Admin"])]])]])
        let x = JsonRender.stringify tree
        let y = JsonDriver.parse x
        show y
        Should.equal y tree

    [<Fact>]
    member this.``SerializeReferencesWithMetadata test``() =
        let tree = 
            Json.Fields(set ["$id",Json.String "1";"$type",Json.String "YourNamespace.Manager, YourAssembly";"Name",Json.String "Mike Manager";"Reportees",Json.Elements [Json.Fields(set ["$id",Json.String "2";"$type",Json.String "YourNamespace.Employee, YourAssembly";"Name",Json.String "Arnie Admin"]);Json.Fields(set ["$id",Json.String "3";"$type",Json.String "YourNamespace.Manager, YourAssembly";"Name",Json.String "Susan Supervisor";"Reportees",Json.Elements [Json.Fields(set ["$ref",Json.String "2"])]])]])
        let x = JsonRender.stringify tree
        let y = JsonDriver.parse x
        show y
        Should.equal y tree


    [<Fact>]
    member this.``SerializeAttributes test``() =
        let tree = 
            Json.Elements [
                Json.Fields(set ["Bedrooms",Json.Int32 2;"BuildDate",Json.String "1890-01-01T00:00:00";"FloorArea",Json.Double 100.0;"StreetAddress",Json.String "221B Baker Street"]);Json.Fields(set ["StreetAddress",Json.String "221B Baker Street"]);Json.Fields(set ["address",Json.String "221B Baker Street"]);Json.Fields(set ["address",Json.String "221B Baker Street";"buildDate",Json.String "1890-01-01T00:00:00"]);Json.Fields(set ["address",Json.String "221B Baker Street";"buildDate",Json.String "new Date(-2524568400000)"])
                ]
        let x = JsonRender.stringify tree
        let y = JsonDriver.parse x
        show y
        Should.equal y tree

    [<Fact>]
    member this.``map name test``() =
        let tree = Json.Fields(set ["Blue",Json.Int32 0;"Green",Json.Int32 0;"Red",Json.Int32 255])
        let y = tree.["Blue"]
        show y
        //Should.equal y tree

    [<Fact>]
    member this.``list index test``() =
        let tree = 
            Json.Elements [
                Json.Fields(set ["Bedrooms",Json.Int32 2;"BuildDate",Json.String "1890-01-01T00:00:00";"FloorArea",Json.Double 100.0;"StreetAddress",Json.String "221B Baker Street"]);Json.Fields(set ["StreetAddress",Json.String "221B Baker Street"]);Json.Fields(set ["address",Json.String "221B Baker Street"]);Json.Fields(set ["address",Json.String "221B Baker Street";"buildDate",Json.String "1890-01-01T00:00:00"]);Json.Fields(set ["address",Json.String "221B Baker Street";"buildDate",Json.String "new Date(-2524568400000)"])
                ]

        let y = tree.[0]
        show y

