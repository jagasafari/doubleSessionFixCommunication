module Connection

type SessionState = Logon | Logout

let connectionState () =
    let mutable state = (Logout, Logout), (Logout, Logout)
    let get () = state
    let update (a1, b1) (a2, b2) = 
        (a1 <> b1 && a1 = a2 && b1 = b2) |> not
    let updateStreaming streamingState = 
        let _, prev = state
        let _, tradingState = prev
        if update prev (streamingState, tradingState)
        then state <- prev, (streamingState, tradingState)
    let updateTrading tradingState =
        let _, prev = state
        let streamingState, _ = prev
        if update prev (streamingState, tradingState) 
        then state <- prev, (streamingState, tradingState)
    get, updateStreaming, updateTrading

type ConnectionMsg = 
    StartStreaming | StartTrading | StopStreaming | StopTrading

type TargetAction = Disconnecting | Connecting

let reactor startStreaming startTrading stopStreaming stopTrading 
    = function
    | StartStreaming -> startStreaming()
    | StartTrading -> startTrading()
    | StopStreaming -> stopStreaming()
    | StopTrading -> stopTrading()
    
let connectionHandle logInfo logError getState reactor () = 
    let logInvalidState () = 
        logError "Invalid connection state. Should not happen!"
    let state = getState ()
    
    match state with
    | (Logon, Logon), (Logon, Logon) -> () 
    | _ -> logInfo state

    match state with
    | (Logon, Logon), (Logon, Logon) -> ()
    | (Logon, Logout), (Logon, Logon) -> ()
    | (Logon, Logout), (Logout, Logout) -> reactor StartStreaming
    | (Logout, Logout), (Logout, Logout) -> reactor StartStreaming
    | (Logout, Logon), (Logout, Logout) -> reactor StartStreaming
    | (Logout, Logout), (Logon, Logout) -> reactor StartTrading
    | (Logout, Logout), (Logout, Logon) -> reactor StopTrading
    | (Logon, Logon), (Logon, Logout) -> reactor StopStreaming
    | (Logon, Logon), (Logout, Logon) -> reactor StopTrading
// hadling state makes below cases impossible, 
// if they happen, then the code is broken
    | (Logout, Logout), (Logon, Logon) -> logInvalidState ()
    | (Logout, Logon), (Logon, Logon) -> logInvalidState ()
    | (Logon, Logon), (Logout, Logout) ->
        logInvalidState (); reactor StartStreaming
    | (Logon, Logout), (Logon, Logout)  -> 
        logInvalidState (); reactor StopStreaming
    | (Logon, Logout), (Logout, Logon)  -> 
        logInvalidState (); reactor StopTrading
    | (Logout, Logon), (Logon, Logout) -> 
        logInvalidState (); reactor StopStreaming
    | (Logout, Logon), (Logout, Logon) ->
        logInvalidState (); reactor StopTrading
