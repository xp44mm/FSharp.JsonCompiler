module FSharp.JsonCompiler.TimeSpanConverter

open System
open FSharp.Literals

let TimeSpanReader = {
    new FSharpReader with
        member _.filter(ty,_) = ty = typeof<TimeSpan>
        member _.read(loopRead, ty, value) =
            value.ToString() 
            |> Json.String
}

let TimeSpanWriter = {
    new FSharpWriter with
        member this.filter(ty:Type, json:Json) = ty = typeof<TimeSpan>
        member this.write(loopWrite:Type -> Json -> obj, ty:Type, json:Json) =
            match json with
            | Json.String s -> box <| TimeSpan.Parse(s)
            | _ -> failwith (Render.stringify json)
}
