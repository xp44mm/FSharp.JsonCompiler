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

### 编译原理

这个解析器分为两个独立的部分，词法分析器，LR上下文无关语法分析。词法分析器首先用.net库的`Regex`正则表达式，匹配文本前缀，生成词法Token的序列，类型为`JsonToken`。语法分析器将词法Token序列，进一步解析成语法树，类型是`Json`。

### 依赖说明

本程序没有使用传统的yacc生成解析代码。而是依赖`ParsingProgram`，位于另一个库`Compiler.Parsing`。解析词法Token序列，生成语法树。前者没有开源。还使用龙书的算法生成解析表，解析表生成程序私有，也没有开源。

传统yacc词法分析，语法分析，过于紧密耦合，所以，我编写了充分利用F#及其语言库的新的编译器程序。JSON解析器是利用这个编译器的一个实践。