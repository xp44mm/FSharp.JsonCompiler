module FSharp.JsonCompiler.ListConverter

open System
open FSharp.Literals

let ListReader = {
    new FSharpReader with
        member _.filter(ty,_) = ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<List<_>>
        member _.read(loopRead, ty, value) =
            let reader = Readers.listReader ty
            let elements = reader value
            let elemType = ty.GenericTypeArguments.[0]
            FSharpReaders.readArrayElements loopRead elemType elements
}

let ListWriter = {
    new FSharpWriter with
        member this.filter(ty:Type, json:Json) = ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<List<_>>
        member this.write(loopWrite:Type -> Json -> obj, ty:Type, json:Json) =
            let elementType = ty.GenericTypeArguments.[0]
            let arrayType = elementType.MakeArrayType()
            let mOfArrayDef = FSharpModules.listModuleType.GetMethod "OfArray"
            let mOfArray = mOfArrayDef.MakeGenericMethod(elementType)
            let arr = loopWrite arrayType json
            mOfArray.Invoke(null, Array.singleton arr)

}
