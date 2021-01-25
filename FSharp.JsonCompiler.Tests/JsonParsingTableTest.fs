namespace FSharp.JsonCompiler.Tests

open Xunit
open Xunit.Abstractions
open System.IO

open FSharpCompiler.Yacc
open FSharp.Literals
open FSharp.JsonCompiler
open FSharp.xUnit

type JsonParsingTableTest(output:ITestOutputHelper) =
    let show res =
        res
        |> Render.stringify
        |> output.WriteLine

    let solutionPath = DirectoryInfo(__SOURCE_DIRECTORY__).Parent.FullName
    let yaccPath = Path.Combine(solutionPath, @"FSharp.JsonCompiler\json.yacc")
    let text = File.ReadAllText(yaccPath)
    let yaccFile = YaccFile.parse text

    [<Fact>]
    member this.``driver data``() =
        //show driver.mainRules
        let mainRules = [
            ["value";"{";"fields";"}"];
            ["value";"[";"values";"]"];
            ["value";"STRING"];
            ["value";"NULL"];
            ["value";"FALSE"];
            ["value";"TRUE"];
            ["value";"CHAR"];
            ["value";"SBYTE"];
            ["value";"BYTE"];
            ["value";"INT16"];
            ["value";"INT32"];
            ["value";"INT64"];
            ["value";"INTPTR"];
            ["value";"UINT16"];
            ["value";"UINT32"];
            ["value";"UINT64"];
            ["value";"UINTPTR"];
            ["value";"BIGINTEGER"];
            ["value";"SINGLE"];
            ["value";"DOUBLE"];
            ["value";"DECIMAL"];
            ["fields";"fields";",";"field"];
            ["fields";"field"];
            ["fields"];
            ["field";"STRING";":";"value"];
            ["values";"values";",";"value"];
            ["values";"value"];
            ["values"]]
        
        Should.equal yaccFile.mainRules mainRules

        Assert.True(yaccFile.precedences.IsEmpty)

    [<Fact>]
    member this.``解决冲突``() =
        // 查看生成的表是否有冲突
        let tbl = AmbiguousTable.create yaccFile.mainRules

        //解析表没有产生式冲突
        let pconflicts = ConflictFactory.productionConflict tbl.ambiguousTable

        Assert.True(pconflicts.IsEmpty)

        // 符号多用警告
        let warning = ConflictFactory.overloadsWarning tbl

        Assert.True(warning.IsEmpty)

        //优先级冲突
        let srconflicts = ConflictFactory.shiftReduceConflict tbl
        
        Assert.True(srconflicts.IsEmpty)


    [<Fact>]
    member this.``测试解析表数据是否为最新``() =
        //所有冲突都解决了，可以生成解析表
        let yacc = ParseTable.create(yaccFile.mainRules, yaccFile.precedences)

        ////解析表数据
        //let result =
        //    [
        //        "let rules = " + Render.stringify yacc.rules
        //        "let kernelSymbols = " + Render.stringify yacc.kernelSymbols
        //        "let parsingTable = " + Render.stringify yacc.parsingTable
        //    ] |> String.concat System.Environment.NewLine
        //output.WriteLine(result)

        //生成的产生式等于源代码中的产生式
        Should.equal yacc.rules JsonParsingTable.rules
        Should.equal yacc.parsingTable JsonParsingTable.parsingTable
        Should.equal yacc.kernelSymbols JsonParsingTable.kernelSymbols


