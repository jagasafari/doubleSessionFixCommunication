module AgentTests

open Xunit
open Swensen.Unquote
open Agent
open Common.TestUtil
open Common.TypeLookUp

[<Fact>]
let ``createAgent:`` () =
    let add, get = mock ()
    let start, post, stop = agent add add "abc" add
    System.Threading.Thread.Sleep 500
    get () =! []
    start ()
    post (Message "ble")
    stop ()
    post (Message "ala")
    stop ()
    System.Threading.Thread.Sleep 500
    get () =!
    [
        "Starting|Agent=abc"
        "ble"
        "Stopping|Agent=abc"
    ]
