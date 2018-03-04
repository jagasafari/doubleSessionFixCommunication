module CurrencyPairSubscriptions

open Client.Model 

let mutateSubscriptionCache 
    set reset remove = function
    | SetCache x -> set x 
    | Reset -> reset () 
    | Remove x -> remove x

let subscriptionCache () =
    let mutable value: Set<_> = Set.empty
    let get () = value
    let set newValue = value <- newValue
    let reset () = value <- Set.empty
    let remove x = value <- value.Remove x
    let mutate = mutateSubscriptionCache set reset remove
    get, mutate

let createSubscriptionCache createAgent =
    ()
