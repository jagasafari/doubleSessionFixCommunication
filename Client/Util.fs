module Util

open Microsoft.FSharp.Reflection
let getUnionCaseName<'T> case =
    (FSharpValue.GetUnionFields(case, typeof<'T>) |> fst).Name

let decrypt value () = value
