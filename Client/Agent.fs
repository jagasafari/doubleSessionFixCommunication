module Agent

type AgentMsg<'a> = Message of 'a | Stop

type AgentLogMessage = 
    | Starting 
    | Stopping 
    | FatalError of string
    | CaughtError of string

let logAgent logInfo logError name msg = 
    let m = sprintf "%A|Agent=%s" msg name
    match msg with
    | Starting | Stopping -> m |> logInfo
    | FatalError _ | CaughtError _ -> m |> logError

let run log initState handle receive =
    let rec loop state = async {
        try 
            let! msg = receive () 
            match msg with
            | Stop -> log Stopping; ()
            | Message x -> 
                let newState = handle state x 
                return! loop newState
        with ex ->
            CaughtError ex.Message |> log
            return! loop state }
    log Starting 
    loop initState

let createAgent log run =
    let mutable agent: MailboxProcessor<_> option = None
    let start () =
        let a = MailboxProcessor.Start(
                    fun inbox -> run inbox.Receive)
        a.Error.Add(
            fun ex -> FatalError ex.Message |> log)
        agent <- Some a
    let post msg = 
        agent |> Option.iter 
            (fun (a: MailboxProcessor<_>) -> a.Post msg)
    let stop () = post Stop
    start, Message >> post, stop 

let agent logInfo logError name initState handle =
    let log = logAgent logInfo logError name
    let runAgent = run log initState handle
    createAgent log runAgent
