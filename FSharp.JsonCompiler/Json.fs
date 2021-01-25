namespace FSharp.JsonCompiler

open System
open System.Numerics

[<RequireQualifiedAccess>]
type Json =
///对象的键值对
| Pairs of Set<string*Json>
///数组的元素
| Elements of Json list
| Null
| False
| True
| String of string
| Char       of char
| SByte      of SByte
| Byte       of Byte
| Int16      of Int16
| Int32      of Int32
| Int64      of Int64
| IntPtr     of IntPtr
| UInt16     of UInt16
| UInt32     of UInt32
| UInt64     of UInt64
| UIntPtr    of UIntPtr
| BigInteger of BigInteger
| Single     of Single
| Double     of Double
| Decimal    of Decimal

    member t.Item with get(idx:int) =
        match t with
        | Json.Elements ls -> ls.[idx]
        | _ -> failwith ""

    member t.Item with get(key:string) =
        match t with
        | Json.Pairs st -> st |> Seq.find(fun(k,_)-> k = key) |> snd
        | _ -> failwith ""
            