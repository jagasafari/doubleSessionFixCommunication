module Tests

open System.Collections.Generic
open Xunit
open Swensen.Unquote
open Connection

[<Fact>]
let ``connectionState: crud`` () =
    let get, updateStreaming, updateTrading = connectionState ()
    get () =! ((Logout, Logout), (Logout, Logout))
    updateStreaming Logon
    get() =! ((Logout, Logout), (Logon, Logout))
    updateTrading Logon
    get() =! ((Logon, Logout), (Logon, Logon))
    updateTrading Logon
    get() =! ((Logon, Logon), (Logon, Logon))

[<Fact>]
let ``connectionState: repeated -> logout, logout`` () =
    let get, updateStreaming, updateTrading = connectionState ()
    updateStreaming Logout
    get () =! ((Logout, Logout), (Logout, Logout))
    updateTrading Logout
    get () =! ((Logout, Logout), (Logout, Logout))
    updateStreaming Logout
    get () =! ((Logout, Logout), (Logout, Logout))
    updateTrading Logout
    get () =! ((Logout, Logout), (Logout, Logout))

[<Fact>]
let ``connectionState: repeated -> logon, logon`` () =
    let get, updateStreaming, updateTrading = connectionState ()
    updateStreaming Logon
    get () =! ((Logout, Logout), (Logon, Logout))
    updateTrading Logon
    get () =! ((Logon, Logout), (Logon, Logon))
    updateTrading Logon
    get () =! ((Logon, Logon), (Logon, Logon))
    updateStreaming Logon
    get () =! ((Logon, Logon), (Logon, Logon))
    updateTrading Logon
    get () =! ((Logon, Logon), (Logon, Logon))

[<Fact>]
let ``connectionState: repeated -> logon, logout`` () =
    let get, updateStreaming, updateTrading = connectionState ()
    updateStreaming Logon
    get () =! ((Logout, Logout), (Logon, Logout))
    updateStreaming Logon
    get () =! ((Logout, Logout), (Logon, Logout))
    updateStreaming Logout
    get () =! ((Logon, Logout), (Logout, Logout))

    
let testResult () =
    let result = List<_>()
    let test expected = (result |> Seq.toList) =! expected
    test, result.Add

let reactorTestData =
    [ StartStreaming; StartTrading; StopStreaming; StopTrading ]
    |> Seq.ofList |> Seq.map (fun x -> [|x|])

let reactorMock add =
    let startStreaming () = add StartStreaming
    let startTrading () = add StartTrading
    let stopStreaming () = add StopStreaming
    let stopTrading () = add StopTrading
    reactor startStreaming startTrading stopStreaming stopTrading   

[<Theory; MemberData("reactorTestData")>]
let ``reactor: cases`` msg =
    let test, add = testResult ()
    reactorMock add msg
    test [msg]

let connectionTestData =
    [
    ((Logout, Logout), (Logout, Logout)), true, Some StartStreaming
    ((Logout, Logout), (Logon, Logout)), true, Some StartTrading
    ((Logout, Logout), (Logout, Logon)), true, Some StopTrading
    ((Logout, Logout), (Logon, Logon)), false, None

    ((Logon, Logout), (Logout, Logout)), true, Some StartStreaming
    ((Logon, Logout), (Logon, Logout)), false, Some StopStreaming
    ((Logon, Logout), (Logout, Logon)), false, Some StopTrading
    ((Logon, Logout), (Logon, Logon)), true, None

    ((Logout, Logon), (Logout, Logout)), true, Some StartStreaming
    ((Logout, Logon), (Logon, Logout)), false, Some StopStreaming
    ((Logout, Logon), (Logout, Logon)), false, Some StopTrading
    ((Logout, Logon), (Logon, Logon)), false, None

    ((Logon, Logon), (Logout, Logout)), false, Some StartStreaming
    ((Logon, Logon), (Logon, Logout)), true, Some StopStreaming
    ((Logon, Logon), (Logout, Logon)), true, Some StopTrading
    ((Logon, Logon), (Logon, Logon)), true, None
    ] |> Seq.ofList
        
[<Fact>]
let ``connectionHandle: `` () =
    let test (state, validState, expected) =
        let test, add = testResult ()
        let testLog, addLog = testResult ()
        let getConnectionState () = state
        let log msg = msg |> sprintf "%A" |> addLog
        connectionHandle 
            log log getConnectionState (reactorMock add) ()
        let getExpected = function | None -> [] | Some value -> [value]
        expected |> getExpected |> test
        let invalid = 
            @"""Invalid connection state. Should not happen!"""
        match state, validState with
        | ((Logon, Logon), (Logon, Logon)), true -> []
        | _, true -> [ (sprintf "%A" state) ]
        | _, false -> [ (sprintf "%A" state); invalid ]
        |> testLog
    connectionTestData |> Seq.iter test
