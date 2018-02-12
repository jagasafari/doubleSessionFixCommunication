module Tests

open System
open System.Collections.Generic
open Xunit
open Swensen.Unquote
open Connection
open Client

let print msg = sprintf "%A" msg

[<Fact>]
[<Trait("Category", "Integration")>]
let ``create socket`` () =
    let configPath = "fix.cfg"
    let start, stop = createSocket configPath
    start ()
    Threading.Thread.Sleep 1000
    stop ()

[<Fact>]
let ``connectionState: crud`` () =
    let get, updateStreaming, updateTrading = 
        connectionState ()
    get () =! ((Off, Off), (Off, Off))
    updateStreaming On
    get() =! ((Off, Off), (On, Off))
    updateTrading On
    get() =! ((On, Off), (On, On))
    updateTrading On
    get() =! ((On, Off), (On, On))

[<Fact>]
let ``connectionState: repeated -> logout, logout`` () =
    let get, updateStreaming, updateTrading = 
        connectionState ()
    updateStreaming Off
    get () =! ((Off, Off), (Off, Off))
    updateTrading Off
    get () =! ((Off, Off), (Off, Off))
    updateStreaming Off
    get () =! ((Off, Off), (Off, Off))
    updateTrading Off
    get () =! ((Off, Off), (Off, Off))

[<Fact>]
let ``connectionState: repeated -> logon, logon`` () =
    let get, updateStreaming, updateTrading = 
        connectionState ()
    updateStreaming On
    get () =! ((Off, Off), (On, Off))
    updateTrading On
    get () =! ((On, Off), (On, On))
    updateTrading On
    get () =! ((On, Off), (On, On))
    updateStreaming On
    get () =! ((On, Off), (On, On))
    updateTrading On
    get () =! ((On, Off), (On, On))

[<Fact>]
let ``connectionState: repeated -> logon, logout`` () =
    let get, updateStreaming, updateTrading = 
        connectionState ()
    updateStreaming On
    get () =! ((Off, Off), (On, Off))
    updateStreaming On
    get () =! ((Off, Off), (On, Off))
    updateStreaming Off
    get () =! ((On, Off), (Off, Off))

    
let testResult () =
    let result = List<string>()
    let add msg = msg |> print |> result.Add
    let test expected = 
        let r =
            result 
            |> Seq.map (fun x -> x.Replace(@"""", ""))
            |> Seq.toList
        r =! expected
    test, add

let reactorTestData =
    [ StartStreaming; StartTrading; StopStreaming; StopTrading ]
    |> Seq.ofList |> Seq.map (fun x -> [|x|])

let reactorMock add =
    let startStreaming () = add StartStreaming
    let startTrading () = add StartTrading
    let stopStreaming () = add StopStreaming
    let stopTrading () = add StopTrading
    react startStreaming startTrading stopStreaming stopTrading   

[<Theory; MemberData("reactorTestData")>]
let ``reactor: cases`` msg =
    let test, add = testResult ()
    reactorMock add msg
    test [print msg]

let connectionTestData =
    [
    ((Off, Off), (Off, Off)), true, Some StartStreaming
    ((Off, Off), (On, Off)), true, Some StartTrading
    ((Off, Off), (Off, On)), true, Some StopTrading
    ((Off, Off), (On, On)), false, None

    ((On, Off), (Off, Off)), true, Some StartStreaming
    ((On, Off), (On, Off)), false, Some StopStreaming
    ((On, Off), (Off, On)), false, Some StopTrading
    ((On, Off), (On, On)), true, None

    ((Off, On), (Off, Off)), true, Some StartStreaming
    ((Off, On), (On, Off)), false, Some StopStreaming
    ((Off, On), (Off, On)), false, Some StopTrading
    ((Off, On), (On, On)), false, None

    ((On, On), (Off, Off)), false, Some StartStreaming
    ((On, On), (On, Off)), true, Some StopStreaming
    ((On, On), (Off, On)), true, Some StopTrading
    ((On, On), (On, On)), false, None
    ] |> Seq.ofList
        
[<Fact>]
let ``connectionHandle: `` () =
    let test (state, validState, expected) =
        let testAssert, add = testResult ()
        let getConnectionState () = state
        let log msg = msg |> print |> add
        let react = reactorMock (print >> add)
        connectionHandle log log getConnectionState react ()
        let invalid = 
            "Invalid connection state. Should not happen!"
        match state, validState, expected with
        | ((On, Off), (On, On)), true, None -> []
        | _, true, None -> [ print state ]
        | _, true, Some value -> 
            [ print state; print value ]
        | _, false, None -> [ print state; invalid ]
        | _, false, Some value -> 
            [ print state; invalid; print value ]
        |> testAssert
    connectionTestData |> Seq.iter test
