module FSharp.JsonCompiler.FSharpModules

open System.Reflection

let fsharpAssembly = Assembly.Load("FSharp.Core")

let listModuleType = fsharpAssembly.GetType("Microsoft.FSharp.Collections.ListModule")
let setModuleType  = fsharpAssembly.GetType("Microsoft.FSharp.Collections.SetModule")
let mapModuleType  = fsharpAssembly.GetType("Microsoft.FSharp.Collections.MapModule")
