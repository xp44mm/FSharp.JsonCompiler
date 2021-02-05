module FSharp.JsonCompiler.JsonDriver

open FSharpCompiler.Parsing

let parser =
    SyntacticParser(
        JsonParsingTable.rules,
        JsonParsingTable.kernelSymbols,
        JsonParsingTable.parsingTable)

let parseTokens tokens =
    let parsingTree = parser.parse(tokens,JsonTokenizer.getTag)
    let expr = JsonTranslation.translateValue parsingTree
    expr

let parse(text:string) =
    let tokens =
        text
        |> JsonTokenizer.tokenize
    let tree = parseTokens tokens
    tree
