module CurrencyPairSubscriptions

open Client.Model 

let mutateSubscriptionCache 
    set clean remove = function
    | Refresh x -> set x 
    | Clean -> clean () 
    | Remove x -> remove x

let subscriptionCache () =
    let mutable value: Set<_> = Set.empty
    let get () = value
    let set newValue = value <- newValue
    let reset () = value <- Set.empty
    let remove x = value <- value.Remove x
    let mutate = mutateSubscriptionCache set reset remove
    get, mutate

let subscription agent =
    let getCache, mutateCache = subscriptionCache ()
    let start, post, stop = 
        agent "Subscription" mutateCache 
    let refresh = Refresh >> post
    let clean () = post Clean
    let remove = Remove >> post
    let stopCache () = clean (); stop ()
    start, stopCache, getCache, refresh, clean, remove
