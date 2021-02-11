module FSharp.JsonCompiler.UnionConverter

open System
open FSharp.Literals
open Microsoft.FSharp.Reflection

let UnionReader = {
    new FSharpReader with
        member _.filter(ty,_) = FSharpType.IsUnion ty
        member _.read(loopRead, ty, value) =
            let reader = DiscriminatedUnion.unionReader ty
            let name,fields = reader value
            let unionFields =
                if Array.isEmpty fields then 
                    // union case is paramless
                    Json.Null
                elif fields.Length = 1 then 
                    // union case is single param
                    loopRead <|| fields.[0]
                else 
                    // union case is tuple
                    FSharpReaders.readTupleFields loopRead fields

            Set.singleton (name,unionFields)
            |> Json.Fields


}

let UnionWriter = {
    new FSharpWriter with
        member this.filter(ty,json) = FSharpType.IsUnion ty
        member this.write(loopWrite,ty,json) =
            match json with
            | Json.Fields fields ->
                let jkey, jvalue = Seq.exactlyOne fields

                let unionCaseInfo =
                    FSharpType.GetUnionCases ty
                    |> Array.find(fun c -> c.Name = jkey)

                let uionFieldTypes =
                    unionCaseInfo.GetFields()
                    |> Array.map(fun info -> info.PropertyType)

                match uionFieldTypes with
                | [||] ->
                    FSharpValue.MakeUnion(unionCaseInfo, Array.empty)
                | [|fieldType|] ->
                    FSharpValue.MakeUnion(unionCaseInfo, Array.singleton(loopWrite fieldType jvalue))
                | _ ->
                    let fields =
                        match jvalue with
                        | Json.Elements elements ->
                            elements
                            |> Array.ofList
                            |> Array.zip uionFieldTypes
                            |> Array.map(fun (t,j) -> loopWrite t j)
                        | _ -> failwith "Json structure does not match"
                    FSharpValue.MakeUnion(unionCaseInfo, fields)
            | _ -> failwith "Json structure does not match"
}
