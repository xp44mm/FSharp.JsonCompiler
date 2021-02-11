module FSharp.JsonCompiler.EnumConverter

open System
open FSharp.Literals
open System.Reflection
open System
open System.Collections.Concurrent
open System.Reflection
open FSharp.Literals

///[<Flags>] Enum 的值
let flagsReader =
    let dic = ConcurrentDictionary<Type, obj -> string[]>(HashIdentity.Structural)
    let valueFactory (ty:Type) =
        let enumUnderlyingType = ty.GetEnumUnderlyingType()
        let zeroPairs,positivePairs =
            Enum.GetNames(ty)
            |> Array.map(fun name ->
                let value =     
                    ty.GetField(
                        name, 
                        BindingFlags.NonPublic ||| BindingFlags.Public ||| BindingFlags.Static
                    ).GetValue(null)
                value, name
            )
            |> Array.partition(fun(v,nm) -> EnumUtils.isGenericZero enumUnderlyingType v)

        let zeroNames = zeroPairs |> Array.map snd

        let reader (inpEnum:obj) =
            let flagNames =
                positivePairs
                |> Array.filter(fun(flagValue,name)-> EnumUtils.genericMask enumUnderlyingType inpEnum flagValue)
                |> Array.map snd

            if Array.isEmpty flagNames then zeroNames else flagNames
        reader

    fun (ty:Type) -> dic.GetOrAdd(ty, Func<_,_> valueFactory)

/// 
let toUInt64 (ty:Type) =
    if ty = typeof<sbyte> then
        unbox<sbyte> >> uint64
    elif ty = typeof<byte> then
        unbox<byte> >> uint64
    elif ty = typeof<int16> then
        unbox<int16> >> uint64
    elif ty = typeof<uint16> then
        unbox<uint16> >> uint64
    elif ty = typeof<int32> then
        unbox<int32> >> uint64
    elif ty = typeof<uint32> then
        unbox<uint16> >> uint64
    elif ty = typeof<int64> then
        unbox<int64> >> uint64
    elif ty = typeof<uint64> then
        unbox<uint64>
    elif ty = typeof<nativeint> then
        unbox<nativeint> >> uint64
    elif ty = typeof<unativeint> then
        unbox<unativeint> >> uint64
    else
        failwith "Unknown Enum Underlying Type."
        
/// 
let fromUInt64 (ty:Type) (src:uint64) =
    src |>
    if ty = typeof<sbyte> then
        sbyte >> box
    elif ty = typeof<byte> then
        byte >> box
    elif ty = typeof<int16> then
        int16 >> box
    elif ty = typeof<uint16> then
        uint16 >> box
    elif ty = typeof<int32> then
        int32 >> box
    elif ty = typeof<uint32> then
        uint16 >> box
    elif ty = typeof<int64> then
        int64 >> box
    elif ty = typeof<uint64> then
        box
    elif ty = typeof<nativeint> then
        nativeint >> box
    elif ty = typeof<unativeint> then
        unativeint >> box
    else
        failwith "Unknown Enum Underlying Type."

let private read (ty:Type) (value:obj) =
    let enumUnderlyingType = ty.GetEnumUnderlyingType()
    let names =
        Enum.GetNames(ty)
        |> Array.map(fun name ->
            let value =     
                ty.GetField(
                    name, 
                    BindingFlags.NonPublic ||| BindingFlags.Public ||| BindingFlags.Static
                ).GetValue(null)
                |> toUInt64 enumUnderlyingType
            value, name
        )
        |> Map.ofArray

    if ty.IsDefined(typeof<FlagsAttribute>,false) then
        let reader = flagsReader ty
        reader value
        |> Array.map(Json.String)
        |> Array.toList
        |> Json.Elements
    else
        Json.String <| names.[toUInt64 enumUnderlyingType value]

let private write (ty:Type) (json:Json) =
    let enumUnderlyingType = ty.GetEnumUnderlyingType()
    let values =
        Enum.GetNames(ty)
        |> Array.map(fun name ->
            let value =     
                ty.GetField(
                    name, 
                    BindingFlags.NonPublic ||| BindingFlags.Public ||| BindingFlags.Static
                ).GetValue(null)
                |> toUInt64 enumUnderlyingType
            name, value
        )
        |> Map.ofArray

    if ty.IsDefined(typeof<FlagsAttribute>,false) then
        match json with
        | Json.Elements flags ->
            flags
            |> List.map(function
                | Json.String flag -> values.[flag] 
                | json -> failwith (Render.stringify json)
            )
            |> List.reduce(|||)
        | _ -> failwith (Render.stringify json)
    else
        match json with
        | Json.String enum -> values.[enum]
        | _ -> failwith (Render.stringify json)
    |> fromUInt64 enumUnderlyingType


let EnumReader = {
    new FSharpReader with
        member _.filter(ty,_) = ty.IsEnum
        member _.read(loopRead, ty, value) = read ty value
}

let EnumWriter = {
    new FSharpWriter with
        member this.filter(ty:Type, json:Json) = ty.IsEnum
        member this.write(loopWrite:Type -> Json -> obj, ty:Type, json:Json) = write ty json
}
