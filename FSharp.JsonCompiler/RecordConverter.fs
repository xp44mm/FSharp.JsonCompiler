module FSharp.JsonCompiler.RecordConverter

open System
open FSharp.Literals
open Microsoft.FSharp.Reflection

let RecordReader = {
    new FSharpReader with
        member _.filter(ty,value) = FSharpType.IsRecord ty
        member _.read(loopRead, ty, value) = 
            let reader = Readers.recordReader ty
            let fields = reader value
    
            fields
            |> Array.map(fun(nm,tp,value) -> nm, loopRead tp value)
            |> Set.ofArray
            |> Json.Fields

}

let RecordWriter = {
    new FSharpWriter with
        member this.filter(ty:Type, json:Json) = FSharpType.IsRecord ty
        member this.write(loopWrite:Type -> Json -> obj, ty:Type, json:Json) =
            match json with
            | Json.Fields fields ->
                let recordFields = FSharpType.GetRecordFields(ty)
                let values =
                    recordFields
                    |> Array.map(fun pi ->
                        let t = pi.PropertyType
                        let jValue = fields |> Seq.find(fun(k,v)->k=pi.Name) |> snd
                        loopWrite t jValue
                    )
                FSharpValue.MakeRecord(ty,values)
            | _ -> failwith "RecordWriter.write()"
}
