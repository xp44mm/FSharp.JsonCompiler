module FSharp.JsonCompiler.JsonCreation

open FSharpCompiler.Parsing

let rec createValue = function
| Interior("value",[Terminal(LEFT_BRACE);fields;Terminal(RIGHT_BRACE)]) ->
    fields
    |> createFields
    |> Set.ofList
    |> Json.Pairs
| Interior("value",[Terminal(LEFT_BRACK);values;Terminal(RIGHT_BRACK)]) ->
    values
    |> createValues
    |> List.rev
    |> Json.Elements
| Interior("value",[Terminal(NULL)]) ->
    Json.Null
| Interior("value",[Terminal(FALSE)]) ->
    Json.False
| Interior("value",[Terminal(TRUE)]) ->
    Json.True
| Interior("value",[Terminal(STRING s)]) ->
    Json.String s
| Interior("value",[Terminal(CHAR s)]) ->
    Json.Char s
| Interior("value",[Terminal(SBYTE x)]) ->
    Json.SByte x
| Interior("value",[Terminal(BYTE x)]) ->
    Json.Byte x
| Interior("value",[Terminal(INT16 x)]) ->
    Json.Int16 x
| Interior("value",[Terminal(UINT16 x)]) ->
    Json.UInt16 x
| Interior("value",[Terminal(INT32 x)]) ->
    Json.Int32 x
| Interior("value",[Terminal(UINT32 x)]) ->
    Json.UInt32 x
| Interior("value",[Terminal(INT64 x)]) ->
    Json.Int64 x
| Interior("value",[Terminal(UINT64 x)]) ->
    Json.UInt64 x
| Interior("value",[Terminal(INTPTR x)]) ->
    Json.IntPtr x
| Interior("value",[Terminal(UINTPTR x)]) ->
    x
    |> Json.UIntPtr
| Interior("value",[Terminal(BIGINTEGER x)]) ->
    x
    |> Json.BigInteger
| Interior("value",[Terminal(SINGLE x)]) ->
    x
    |> Json.Single
| Interior("value",[Terminal(DOUBLE x)]) ->
    x
    |> Json.Double
| Interior("value",[Terminal(DECIMAL x)]) ->
    x
    |> Json.Decimal
| never -> failwithf "%A"  <| never.get_firstLevel()

and createFields = function
| Interior("fields",[Interior("fields",_) as ls; comma; field]) ->
    createField field :: createFields ls
| Interior("fields",[field]) ->
    [createField field]
| Interior("fields",[]) ->
    []
| never -> failwithf "%A"  <| never.get_firstLevel()

and createField = function
| Interior("field",[Terminal(STRING s); colon; value]) ->
    (s, createValue value)
| never -> failwithf "%A"  <| never.get_firstLevel()

and createValues = function
| Interior("values",[Interior("values",_) as ls; comma; value]) ->
    createValue value :: createValues ls
| Interior("values",[value]) ->
    [createValue value]
| Interior("values",[]) ->
    []
| never -> failwithf "%A" <| never.get_firstLevel()
