﻿[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module FSharp.JsonCompiler.Json

let parse(text:string) = JsonDriver.parse text

let stringify tree = JsonRender.stringify tree


