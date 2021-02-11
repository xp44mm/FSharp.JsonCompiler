module FSharp.JsonCompiler.ArrayConverter

open System
open FSharp.Literals
open Microsoft.FSharp.Reflection

let ArrayReader = {
    new FSharpReader with
        member _.filter(ty,value) = ty.IsArray && ty.GetArrayRank() = 1
        member _.read(loopRead, ty, value) = 
            let reader = Readers.arrayReader ty
            let elements = reader value
            let elemType = ty.GetElementType()
            FSharpReaders.readArrayElements loopRead elemType elements

}

let ArrayWriter = {
    new FSharpWriter with
        member this.filter(ty:Type, json:Json) = ty.IsArray && ty.GetArrayRank() = 1
        member this.write(loopWrite:Type -> Json -> obj, ty:Type, json:Json) =
            match json with
            | Json.Elements elements ->
                let elementType = ty.GetElementType()
                let arr = (Array.CreateInstance:Type*int->Array)(elementType,elements.Length)
                elements
                |> List.map(fun e -> loopWrite elementType e)
                |> List.iteri(fun i v ->
                    arr.SetValue(v, i)
                )
                box arr
            | _ -> failwith "ArrayWriter.write()"
}
