module Tests

open System
open Xunit
open Swensen.Unquote

type SessionState = Logon | Logout

let communicationState () =
    let mutable state = (Logout, Logout), (Logout, Logout)
    let get () = state
    let updateStreaming streamingState = 
        let _, prev = state
        let _, tradingState = prev
        state <- (prev, (streamingState, tradingState))
    let updateTrading tradingState =
        let _, prev = state
        let streamingState, _ = prev
        state <- (prev, (streamingState, tradingState))
    get, updateStreaming, updateTrading

[<Fact>]
let ``communicationState: crud`` () =
    let get, updateStreaming, updateTrading = 
        communicationState ()
    get () =! ((Logout, Logout), (Logout, Logout))
    updateStreaming Logon
    get() =! ((Logout, Logout), (Logon, Logout))
    updateTrading Logon
    get() =! ((Logon, Logout), (Logon, Logon))
    updateTrading Logon
    get() =! ((Logon, Logon), (Logon, Logon))
