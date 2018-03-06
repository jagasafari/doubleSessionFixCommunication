module CurrencyPairSubscriptionsTests

open Xunit
open Swensen.Unquote
open Client.Model
open CurrencyPairSubscriptions
open Common.TestUtil
open Agent

let set = Set.ofList

[<Fact>]
let ``subscription:`` () =
    let add, get = mock ()
    let req _ = ()
    let rej _ = ()
    let pull () = []
    let isLogon () = true
    let agent' = agent add add
    let start, stop, refresh, clean, remove, unsubscribeAll
        = subscription agent' isLogon pull req rej
    get () =! []

let refreshTestData =
    [
    ([2;5],[2;5],[2;5],[-1])
    ([],[],[],[-1])
    ([4],[],[4],[-1;4])
    ([],[9],[],[-1;-9])
    ([],[9;7;2],[],[-1;-2;-7;-9])
    ([4;7],[9;7;2],[4;7],[-1;4;-2;-9])
    ([9;2;4;7],[9;7;2],[9;2;4;7],[-1;4])
    ] |> cast4TestData

[<Theory; MemberData("refreshTestData")>]
let ``refresh: `` latest current expectedState expected =
    let add, get = mock ()
    let subscribe x = add x
    let unsubscribe x = add (x * -1)
    let pull () = add -1; latest
    let state = set current
    refresh pull subscribe unsubscribe state 
    =! (set expectedState)
    get () =! expected

[<Fact>]
let ``subscriptionHandle: `` () =
    let refresh state = set [2]
    let state = set [2;4;9;0] 
    let unsb _ = ()
    let handle = subscriptionHandle refresh unsb state
    handle Clean =! Set.empty
    handle (Remove 4) =! (set [2;9;0])
    handle (Remove 1) =! (set [2;4;9;0])
    handle Refresh =! (set [2])
    handle UnsubscribeAll =! Set.empty
