module FSharp.JsonCompiler.JsonTranslation

open FSharpCompiler.Parsing

let rec translateValue = function
| Interior("value",[Terminal(LEFT_BRACE);fields;Terminal(RIGHT_BRACE)]) ->
    fields
    |> translateFields
    |> Set.ofList
    |> Json.Fields
| Interior("value",[Terminal(LEFT_BRACK);values;Terminal(RIGHT_BRACK)]) ->
    values
    |> translateValues
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

and translateFields = function
| Interior("fields",[Interior("fields",_) as ls; comma; field]) ->
    translateField field :: translateFields ls
| Interior("fields",[field]) ->
    [translateField field]
| Interior("fields",[]) ->
    []
| never -> failwithf "%A"  <| never.get_firstLevel()

and translateField = function
| Interior("field",[Terminal(STRING s); colon; value]) ->
    (s, translateValue value)
| never -> failwithf "%A"  <| never.get_firstLevel()

and translateValues = function
| Interior("values",[Interior("values",_) as ls; comma; value]) ->
    translateValue value :: translateValues ls
| Interior("values",[value]) ->
    [translateValue value]
| Interior("values",[]) ->
    []
| never -> failwithf "%A" <| never.get_firstLevel()
