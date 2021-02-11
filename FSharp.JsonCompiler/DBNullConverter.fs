module FSharp.JsonCompiler.DBNullConverter

open System

let DBNullReader = {
    new FSharpReader with
        member _.filter(ty,value) = ty = typeof<DBNull> || DBNull.Value.Equals value
        member _.read(loopRead, ty, value) = Json.Null
}

let DBNullWriter = {
    new FSharpWriter with
        member this.filter(ty:Type, json:Json) = ty = typeof<DBNull>
        member this.write(loopWrite:Type -> Json -> obj, ty:Type, json:Json) = box DBNull.Value
}
