module Common.TestUtil

open System
open System.Collections.Generic

let mock () =
    let result = List<_>()
    let add msg = result.Add msg
    let get () = result |> Seq.toList
    add, get 

let testCmd () =
    let mutable result = String.Empty
    let add msg = result <- msg
    let get () = result
    add, get 

let cast2TestData data =
    let castTuple (x, y) = [|x:>obj; y:>obj|]
    List.toSeq data |> Seq.map castTuple

let cast3TestData data =
    let castTuple (x, y, z) = [|x:>obj; y:>obj; z:>obj|]
    List.toSeq data |> Seq.map castTuple

