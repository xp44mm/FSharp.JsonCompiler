module FSharp.JsonCompiler.SetConverter

open System
open FSharp.Literals

let SetReader = {
    new FSharpReader with
        member _.filter(ty,_) = ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Set<_>>
        member _.read(loopRead, ty, value) =
            let reader = Readers.setReader ty
            let elements = reader value
            let elementType = ty.GenericTypeArguments.[0]
            FSharpReaders.readArrayElements loopRead elementType elements

}

let SetWriter = {
    new FSharpWriter with
        member this.filter(ty:Type, json:Json) = ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Set<_>>
        member this.write(loopWrite:Type -> Json -> obj, ty:Type, json:Json) =
            let elementType = ty.GenericTypeArguments.[0]
            let arrayType = elementType.MakeArrayType()
            let mOfArrayDef = FSharpModules.setModuleType.GetMethod "OfArray"
            let mOfArray = mOfArrayDef.MakeGenericMethod(elementType)
            let arr = loopWrite arrayType json
            mOfArray.Invoke(null, Array.singleton arr)

}
