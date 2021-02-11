module FSharp.JsonCompiler.DateTimeOffsetConverter

open System
open FSharp.Literals

let DateTimeOffsetReader = {
    new FSharpReader with
        member _.filter(ty,_) = ty = typeof<DateTimeOffset>
        member _.read(loopRead, ty, value) =
            value.ToString()
            |> Json.String
}

let DateTimeOffsetWriter = {
    new FSharpWriter with
        member this.filter(ty:Type, json:Json) = ty = typeof<DateTimeOffset>
        member this.write(loopWrite:Type -> Json -> obj, ty:Type, json:Json) =
            match json with
            | Json.String s -> box <| DateTimeOffset.Parse(s)
            | _ -> failwith (Render.stringify json)
}
