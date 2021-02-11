module FSharp.JsonCompiler.NullableConverter

open System
open FSharp.Literals

let NullableReader = {
    new FSharpReader with
        member _.filter(ty,value) = ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Nullable<_>>
        member _.read(loopRead, ty, value) = 
            if value = null then
                Json.Null
            else
                let underlyingType = ty.GenericTypeArguments.[0]
                loopRead underlyingType value
}

let NullableWriter = {
    new FSharpWriter with
        member this.filter(ty:Type, json:Json) =
            ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Nullable<_>>
        member this.write(loopWrite:Type -> Json -> obj, ty:Type, json:Json) =
            match json with
            | Json.Null -> null
            | _ ->
                let underlyingType = ty.GenericTypeArguments.[0]
                loopWrite underlyingType json
}
