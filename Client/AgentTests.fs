module AgentTests

open Xunit
open Swensen.Unquote
open Agent
open Common.TestUtil

[<Fact>]
let ``createAgent:`` () =
    let add, get = mock ()
    let handle _ m = add m
    let start, post, stop = agent add add "abc" () handle
    start ()
    post ("ble")
    stop ()
    post ("ala")
    stop ()
    System.Threading.Thread.Sleep 500
    get () =!
    [
        "Starting|Agent=abc"
        "ble"
        "Stopping|Agent=abc"
    ]
