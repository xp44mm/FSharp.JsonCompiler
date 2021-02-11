module FSharp.JsonCompiler.JsonDriver

open FSharpCompiler.Parsing

let parser =
    SyntacticParser(
        JsonParsingTable.rules,
        JsonParsingTable.kernelSymbols,
        JsonParsingTable.parsingTable)

let parseTokens tokens =
    let parsingTree = parser.parse(tokens,JsonTokenizer.getTag)
    JsonTranslation.translateValue parsingTree

let parse(text:string) =
    let tokens = JsonTokenizer.tokenize text
    parseTokens tokens
