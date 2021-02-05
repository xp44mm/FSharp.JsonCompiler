module FSharp.JsonCompiler.FSharpToJson

open System
open Microsoft.FSharp.Reflection

open FSharp.Literals

let rec objectToJson (ty:Type) (obj:obj) =
    if ty = typeof<bool> then
        let b = unbox<bool> obj
        if b then Json.True else Json.False

    elif ty = typeof<sbyte> then
        let value = unbox<sbyte> obj
        Json.SByte value

    elif ty = typeof<byte> then
        let value = unbox<byte> obj
        Json.Byte value

    elif ty = typeof<int16> then
        let value = unbox<int16> obj
        Json.Int16 value

    elif ty = typeof<uint16> then
        let value = unbox<uint16> obj
        Json.UInt16 value

    elif ty = typeof<int> then
        let value = unbox<int> obj
        Json.Int32 value

    elif ty = typeof<uint32> then
        let value = unbox<uint32> obj
        Json.UInt32 value

    elif ty = typeof<int64> then
        let value = unbox<int64> obj
        Json.Int64 value

    elif ty = typeof<uint64> then
        let value = unbox<uint64> obj
        Json.UInt64 value

    elif ty = typeof<nativeint> then
        let value = unbox<nativeint> obj
        Json.IntPtr value

    elif ty = typeof<unativeint> then
        let value = unbox<unativeint> obj
        Json.UIntPtr value

    elif ty = typeof<single> then
        let value = unbox<single> obj
        Json.Single value

    elif ty = typeof<float> then
        let value = unbox<float> obj
        Json.Double value

    elif ty = typeof<decimal> then
        let value = unbox<decimal> obj
        Json.Decimal value

    elif ty = typeof<bigint> then
        let value = unbox<bigint> obj
        Json.BigInteger value

    elif ty = typeof<char> then
        unbox<char> obj
        |> Json.Char

    elif ty = typeof<string> then
        unbox<string> obj
        |> Json.String

    elif ty = typeof<Guid> then
        let outp = Render.stringifyNullableType ty obj
        Json.String outp

    elif ty = typeof<TimeSpan> then
        let outp = Render.stringifyNullableType ty obj
        Json.String outp

    elif ty = typeof<DateTimeOffset> then
        let outp = Render.stringifyNullableType ty obj
        Json.String outp

    elif ty = typeof<DateTime> then
        let outp = Render.stringifyNullableType ty obj
        Json.String outp

    elif ty.IsEnum then
        let outp = Render.stringifyNullableType ty obj
        Json.String outp

    elif ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Nullable<_>> then
        if obj = null then
            Json.Null
        else
            let underlyingType = ty.GenericTypeArguments.[0]
            objectToJson underlyingType obj

    elif ty.IsArray && ty.GetArrayRank() = 1 then
        let reader = Readers.arrayReader ty
        let elements = reader obj
        let elemType = ty.GetElementType()

        arrayToJson elemType elements

    elif ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<List<_>> then
        //一定無需加括號
        let reader = Readers.listReader ty
        let elements = reader obj
        let elemType = ty.GenericTypeArguments.[0]

        arrayToJson elemType elements

    elif ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Set<_>> then
        let reader = Readers.setReader ty
        let elements = reader obj
        let elementType = ty.GenericTypeArguments.[0]
        arrayToJson elementType elements

    elif ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Map<_,_>> then
        let reader = Readers.mapReader ty
        let elements = reader obj
        let tupleType = FSharpType.MakeTupleType(ty.GenericTypeArguments)
        arrayToJson tupleType elements

    elif FSharpType.IsTuple ty then
        let reader = Readers.tupleReader ty
        let elements = reader obj
        let elementTypes = FSharpType.GetTupleElements(ty)

        Array.zip elementTypes elements
        |> tupleToJson

    elif FSharpType.IsUnion ty then
        let reader = DiscriminatedUnion.unionReader ty
        let name,fields = reader obj
        if ty.GetGenericTypeDefinition() = typedefof<Option<_>> then
            if name = "None" then
                Json.Null
            else
                let ftype,field = fields.[0]
                objectToJson ftype field
        else
            (name,tupleToJson fields)
            |> Set.singleton
            |> Json.Pairs

    elif FSharpType.IsRecord ty then
        let reader = Readers.recordReader ty
        let fields = reader obj

        fields
        |> Array.map(fun(nm,tp,value) -> nm, objectToJson tp value)
        |> Set.ofArray
        |> Json.Pairs

    elif obj = null then
        Json.Null
    elif ty = typeof<obj> && obj.GetType() <> typeof<obj> then
        objectToJson (obj.GetType()) obj
    else
        Json.String (Render.stringify obj)

and arrayToJson elemType (elements:obj[]) =
    let ls =
        elements
        |> List.ofArray
        |> List.map(objectToJson elemType)

    Json.Elements ls

and tupleToJson fields =
    fields
    |> List.ofArray
    |> List.map(fun(ftype,field)-> objectToJson ftype field)
    |> Json.Elements

