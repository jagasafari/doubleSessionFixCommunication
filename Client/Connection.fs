module Connection

type SessionState = On | Off

let connectionState () =
    let mutable state = (Off, Off), (Off, Off)
    let get () = state
    let updateStreaming streamingState = 
        let _, prev = state
        let prevStreamingState, tradingState = prev
        if prevStreamingState <> streamingState
        then state <- prev, (streamingState, tradingState)
    let updateTrading tradingState =
        let _, prev = state
        let streamingState, prevTradingState = prev
        if prevTradingState <> tradingState
        then state <- prev, (streamingState, tradingState)
    get, updateStreaming, updateTrading

type ConnectionMsg = 
    StartStreaming | StartTrading | StopStreaming | StopTrading

let react startStreaming startTrading stopStreaming stopTrading 
    = function
    | StartStreaming -> startStreaming()
    | StartTrading -> startTrading()
    | StopStreaming -> stopStreaming()
    | StopTrading -> stopTrading()

let recoverFromImpossible react = function
    | (On, On), (On, On) -> ()
    | (Off, Off), (On, On) -> ()
    | (Off, On), (On, On) -> ()
    | (On, On), (Off, Off) -> react StartStreaming
    | (On, Off), (On, Off)  -> react StopStreaming
    | (On, Off), (Off, On)  -> react StopTrading
    | (Off, On), (On, Off) -> react StopStreaming
    | (Off, On), (Off, On) -> react StopTrading   
    | _ -> ()

let connectionHandle logInfo logError getState react () = 
    let logInvalidState () = 
        logError "Invalid connection state. Should not happen!"
    let state = getState ()
    
    match state with
    | (On, Off), (On, On) -> () 
    | _ -> logInfo state

    match state with
    | (On, Off), (On, On) -> ()
    | (On, Off), (Off, Off) -> react StartStreaming
    | (Off, Off), (Off, Off) -> react StartStreaming
    | (Off, On), (Off, Off) -> react StartStreaming
    | (Off, Off), (On, Off) -> react StartTrading
    | (Off, Off), (Off, On) -> react StopTrading
    | (On, On), (On, Off) -> react StopStreaming
    | (On, On), (Off, On) -> react StopTrading
    | _ -> 
        logInvalidState () 
        recoverFromImpossible react state
