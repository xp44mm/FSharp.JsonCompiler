namespace FSharp.JsonCompiler

open Xunit
open Xunit.Abstractions
open System
open FSharp.Literals
open FSharp.xUnit


type NullableTest(output: ITestOutputHelper) =
    /// 结论：primitive type 装箱后可以强制转化为 nullable类型。
    [<Fact>]
    member this.``prameterless constructor``() =
        let nullableType = typeof<Nullable<int>>
        // 不能使用这个方法，直接返回空，即可。
        Assert.Throws<System.NullReferenceException>(fun () -> 
            let o = Activator.CreateInstance(nullableType)
            // Prove that we created a Person.
            output.WriteLine("A {0} object has been created", o.GetType().Name)
        )

    [<Fact>]
    member this.``non null constructor``() =
        let nullableType = typeof<Nullable<int>>
        let o = Activator.CreateInstance(nullableType, [| box 1 |])
        // Prove that we created a Person.
        output.WriteLine("A {0} object has been created", o.GetType().Name)


    [<Fact>]
    member this.``read from nullable to json``() =
        let x = Nullable 3
        let y = FSharpReaders.mainRead [|NullableConverter.NullableReader|] typeof<Nullable<int>> x
        Should.equal y <| Json.Int32 3

    [<Fact>]
    member this.``write from json to fsharp``() =
        let x = Json.Int32 3
        let y = FSharpWriters.mainWrite [|NullableConverter.NullableWriter|] typeof<Nullable<int>> x
        Should.equal y <| box (Nullable 3)

    [<Fact>]
    member this.``box nullable``() =        
        let x = box 3
        Should.equal x (box (Nullable 3))

    [<Fact>]
    member this.``unbox nullable``() =        
        let x = box 3
        let y = x |> unbox<Nullable<int>>
        Should.equal y (Nullable 3)

    [<Fact>]
    member this.``cast nullable``() =
        let x = box 3
        let yy = x :?> Nullable<int>
        Should.equal yy (Nullable 3)

