module FSharp.JsonCompiler.OptionConverter

open System
open FSharp.Literals
open Microsoft.FSharp.Reflection

let OptionReader = {
    new FSharpReader with
        member _.filter(ty,value) = ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Option<_>>
        member _.read(loopRead, ty, value) = 
            if value = null then
                Json.Null
            else
                let reader = DiscriminatedUnion.unionReader ty
                let _,fields = reader value
                let ftype,fvalue = fields.[0]
                loopRead ftype fvalue

}

let OptionWriter = {
    new FSharpWriter with
        member this.filter(ty:Type, json:Json) =
            ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Option<_>>
        member this.write(loopWrite:Type -> Json -> obj, ty:Type, json:Json) =
            match json with
            | Json.Null -> box None
            | jvalue ->
                let unionCaseInfo =
                    FSharpType.GetUnionCases ty
                    |> Array.find(fun c -> c.Name = "Some")

                let uionFieldType =
                    unionCaseInfo.GetFields()
                    |> Array.map(fun info -> info.PropertyType)
                    |> Array.exactlyOne

                FSharpValue.MakeUnion(unionCaseInfo, Array.singleton(loopWrite uionFieldType jvalue))
}
