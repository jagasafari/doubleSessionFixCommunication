module CurrencyPairSubscriptionsTests

open Xunit
open Swensen.Unquote
open Client.Model
open Agent
open CurrencyPairSubscriptions
open Common.TestUtil
open System.Threading

let set = Set.ofList

[<Fact>]
let ``subscriptionCache: state changes`` () =
    let get, mutate = subscriptionCache ()
    let test x = get () =! (set x)
    test []
    Refresh (["1";"4"]|>set) |> mutate
    test ["1";"4"]
    Remove "4" |> mutate
    test ["1"]
    Clean |> mutate
    test []

[<Fact>]
let ``subscription api`` () =
    let add, get = mock ()
    let agent' = agent add add
    let start, stop, getCache, refresh, clean, remove =
        subscription agent'
    let test x = getCache () =! (set x)
    remove "a"
    test []
    refresh (Set.empty.Add "v")
    test []
    start ()
    refresh (Set.empty.Add "r")
    Thread.Sleep 200
    test ["r"]
    stop ()
    Thread.Sleep 50
    test []
