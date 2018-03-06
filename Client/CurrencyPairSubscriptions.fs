module CurrencyPairSubscriptions

open Client.Model 

let refresh pull subscribe unsubscribe state =
    let latest = pull () |> Set.ofList
    let toRequest = Set.difference latest state
    let toReject = Set.difference state latest
    match toRequest.IsEmpty, toReject.IsEmpty with
    | true, true -> state
    | _ ->
        toRequest |> Set.iter subscribe
        toReject |> Set.iter unsubscribe
        latest

let subscriptionHandle refresh unsubscribe state 
    = function
    | Refresh -> refresh state
    | Clean -> Set.empty
    | Remove x -> state |> Set.remove x
    | UnsubscribeAll -> 
        state |> Set.iter unsubscribe
        Set.empty

let subscription agent isLogon pull subscribe unsubscribe =
    let refresh' = refresh pull subscribe unsubscribe
    let handle = subscriptionHandle refresh' unsubscribe
    let start, post, stop = 
        agent "Sunscription" Set.empty handle
    let refreshSubscription () = 
        if isLogon () then post Refresh
    let clean () = post Clean
    let remove x = post (Remove x)
    let unsubscribeAll () = post UnsubscribeAll

    start, 
    stop, 
    refreshSubscription, 
    clean, 
    remove, 
    unsubscribeAll
