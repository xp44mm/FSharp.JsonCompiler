namespace FSharp.JsonCompiler

open System
open FSharp.Literals
open Microsoft.FSharp.Reflection
open System.Numerics
open System.Reflection
open Microsoft.FSharp.Core

/// write to fsharp value from json
type FSharpWriter =
    abstract filter: targetType:Type * json:Json -> bool
    abstract write: loopWrite:(Type -> Json -> obj) * targetType:Type * json:Json -> obj

module FSharpWriters =
    let assertType<'t> (ty:Type) (value:obj) =
        let t = typeof<'t>
        if ty = t then value else failwithf "type should be `%s`"  t.Name

    ///
    let fallbackWrite (loopWrite:Type -> Json -> obj, ty:Type, json:Json) =
        match json with
        | Json.Null  -> null
        | Json.False -> box false    |> assertType<bool> ty
        | Json.True  -> box true     |> assertType<bool> ty
        | Json.Char       x -> box x |> assertType<char> ty
        | Json.String     x -> box x |> assertType<string> ty
        | Json.SByte      x -> box x |> assertType<sbyte> ty
        | Json.Byte       x -> box x |> assertType<byte> ty
        | Json.Int16      x -> box x |> assertType<int16> ty
        | Json.Int32      x -> box x |> assertType<int32> ty
        | Json.Int64      x -> box x |> assertType<int64> ty
        | Json.IntPtr     x -> box x |> assertType<IntPtr> ty
        | Json.UInt16     x -> box x |> assertType<uint16> ty
        | Json.UInt32     x -> box x |> assertType<uint32> ty
        | Json.UInt64     x -> box x |> assertType<uint64> ty
        | Json.UIntPtr    x -> box x |> assertType<UIntPtr> ty
        | Json.BigInteger x -> box x |> assertType<BigInteger> ty
        | Json.Single     x -> box x |> assertType<Single> ty
        | Json.Double     x -> box x |> assertType<float> ty
        | Json.Decimal    x -> box x |> assertType<decimal> ty
        | Json.Fields fields -> failwith "not allowed type"

        | Json.Elements (elements) ->
            if ty.IsArray then
                let elementType = ty.GetElementType()
                let arr = (Array.CreateInstance:Type*int->Array)(elementType,elements.Length)
                elements
                |> List.map(fun e -> loopWrite elementType e)
                |> List.iteri(fun i v ->
                    arr.SetValue(v, i)
                )
                box arr
            elif FSharpType.IsTuple ty then
                let tps = FSharpType.GetTupleElements(ty)
                let elements = Array.ofList elements
                let values =
                    Array.zip tps elements
                    |> Array.map(fun(tp,json)-> loopWrite tp json)
                FSharpValue.MakeTuple(values,ty)
            else failwith "not allowed type"

    /// convert from json to obj.
    let rec mainWrite (writers:FSharpWriter[]) (ty:Type) (json:Json) =
        let write =
            writers
            |> Array.tryFind(fun w -> w.filter(ty,json))
            |> Option.map(fun w -> w.write)
            |> Option.defaultValue fallbackWrite

        write(mainWrite writers, ty, json)

