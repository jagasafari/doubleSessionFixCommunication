module Common.TypeLookUp

open System
open System.IO
open System.Reflection

let getStaticMembers (t: Type) =
    t.GetMembers(BindingFlags.Public|||BindingFlags.Static)

let getInstanceMembers (t: Type) =
    t.GetMembers(
        BindingFlags.Public|||BindingFlags.Instance)

let getTypeFileName (t: Type) =
    let c = [|'`'|]
    (t.Name.Split(c)).[0]
    
let append (t: Type) fileName theme getMembers =
    let themeLine name = 
        sprintf "###########%s#########" name
    File.AppendAllLines(fileName, [|themeLine theme|])
    let members =
        t |> getMembers |> Array.map (sprintf "%A")
    File.AppendAllLines(fileName, members)

let writeType (t: Type) =
    let fileName = getTypeFileName t
    if File.Exists fileName 
    then File.Delete fileName
    append t fileName "Static" getStaticMembers
    append t fileName "Instance" getInstanceMembers
