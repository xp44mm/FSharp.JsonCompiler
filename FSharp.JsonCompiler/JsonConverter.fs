module FSharp.JsonCompiler.JsonConverter

open System
open FSharp.Reflection

/// convert from json to obj.
let rec toObject (tp:Type) (json:Json) =
    match json with
    | Json.Pairs properties -> 
        let fields = FSharpType.GetRecordFields(tp)
        let values =
            fields
            |> Array.map(fun pi -> 
                let t = pi.PropertyType
                let jsonValue = properties |> Seq.find(fun(k,v)->k=pi.Name) |> snd
                toObject t jsonValue
            )
        FSharpValue.MakeRecord(tp,values)

    | Json.Elements (elements) -> 
        //根据类型可能tuple, 或array
        if FSharpType.IsTuple tp then
            let tps = FSharpType.GetTupleElements(tp)
            let elements = Array.ofList elements
            let values = 
                Array.zip tps elements
                |> Array.map(fun(tp,json)-> toObject tp json)
            FSharpValue.MakeTuple(values,tp)
        elif tp.IsArray then
            let t = tp.GetElementType()
            let arr = (Array.CreateInstance:Type*int->Array)(t,elements.Length)
            elements
            |> List.map(fun e -> toObject t e)
            |> List.iteri(fun i v ->
                arr.SetValue(v, i)
            )
            box arr
        else failwith "not allowed type"
            
    | Json.Null  -> null
    | Json.False -> box false
    | Json.True  -> box true
    | Json.String     x -> box x
    | Json.Char       x -> box x
    | Json.SByte      x -> box x
    | Json.Byte       x -> box x
    | Json.Int16      x -> box x
    | Json.Int32      x -> box x
    | Json.Int64      x -> box x
    | Json.IntPtr     x -> box x
    | Json.UInt16     x -> box x
    | Json.UInt32     x -> box x
    | Json.UInt64     x -> box x
    | Json.UIntPtr    x -> box x
    | Json.BigInteger x -> box x
    | Json.Single     x -> box x
    | Json.Double     x -> box x
    | Json.Decimal    x -> box x
    
/// convert form json to typed instance.
let convert<'a> (json:Json) = toObject (typeof<'a>) json :?> 'a