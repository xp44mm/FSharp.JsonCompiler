# FSharp.JsonCompiler

FSharp.JsonCompiler is a serializer for converting between FSharp structural data tree and JSON.

This code is good for situations where you are only interested in getting values from JSON, you don't have a class to serialize or deserialize to, or the JSON is radically different from your class and you need to manually read and write from your objects.

### Installation Instructions

The recommended way to get `FSharp.JsonCompiler` is to use NuGet. The following packages are provided and maintained in the public NuGet Gallery.

### Get Started

The core data type is a DU that it represent a syntax tree:

```F#
[<RequireQualifiedAccess>]
type Json =
| Map of Map<string,Json>
| List of Json []
| Null
| False
| True
| String of string
| Char       of char
| SByte      of SByte
| Byte       of Byte
| Int16      of Int16
| Int32      of Int32
| Int64      of Int64
| IntPtr     of IntPtr
| UInt16     of UInt16
| UInt32     of UInt32
| UInt64     of UInt64
| UIntPtr    of UIntPtr
| BigInteger of BigInteger
| Single     of Single
| Double     of Double
| Decimal    of Decimal

```

FSharp.JsonCompiler is to convert between this syntax tree to and from JSON string.

```F#
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
show x

let tree = Json.Map(Map.ofList [
    "ExpiryDate",Json.String "2008-12-28T00:00:00";
    "Name",Json.String "Apple";
    "Price",Json.Double 3.99;
    "Sizes",Json.List [|
        Json.String "Small";
        Json.String "Medium";
        Json.String "Large"|]])
Should.equal y tree
```

The `x` result is compact format:

```F#
"""
{"ExpiryDate":"2008-12-28T00:00:00","Name":"Apple","Price":3.99,"Sizes":["Small","Medium","Large"]}
"""
```

When you get a Json tree, you can navigate it to get a node as you need.

```F#
let tree = Json.Map(Map.ofList [
    "ExpiryDate",Json.String "2008-12-28T00:00:00";
    "Name",Json.String "Apple";
    "Price",Json.Double 3.99;
    "Sizes",Json.List [|
        Json.String "Small";
        Json.String "Medium";
        Json.String "Large"|]])

let name = tree.["Name"]
Should.equal name (Json.String "Apple")

let size0 = tree.["Sizes"].[0]
Should.equal size0 (Json.String "Small")

```

### See Also

FSharp.Literals uses .NET and F# reflection to walk the structure of values so as to build a formatted representation of the value.