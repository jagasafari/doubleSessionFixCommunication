module Socket 

open System 
open System.IO
open QuickFix
open Server.Model
open Prices

let app react =
    { new IApplication with
        member this.OnCreate(sessionId) = OnCreate sessionId |> react
        member this.FromAdmin(msg, sessionId) = FromAdmin msg |> react
        member this.ToAdmin(msg, sessionId) = ToAdmin msg |> react
        member this.OnLogon(sessionId) = OnLogon sessionId |> react    
        member this.OnLogout(sessionId) = OnLogout sessionId |> react    
        member this.FromApp(msg, sessionId) = FromApp msg |> react
        member this.ToApp(msg, sessionId) = ToApp msg |> react
    }

let logFactory react =
    { new ILogFactory with
        member this.Create(sessionId) =
            { new ILog with
                member this.Clear() = ()
                member this.Dispose() = ()
                member this.OnEvent(e) = OnEvent e |> react
                member this.OnOutgoing(msg) = OnOutgoing msg |> react
                member this.OnIncoming(msg) = OnIncoming msg |> react
            } 
    }

let applicationHandle setSession = function
    | OnLogon sessionId -> Some sessionId |> setSession
    | OnLogout _ -> setSession None
    | OnCreate _ | ToAdmin _ | FromAdmin _ | ToApp _ | FromApp _ -> ()

let sessionState () =
    let mutable sessionId : SessionID option = None
    let get () = sessionId
    let set id = sessionId <- id
    get, set

let createSocket configPath (logFixMsg, logAppMsg) =
    let getSettings () =
        let config = configPath |> File.ReadAllText
        use configReader = new StringReader(config)
        SessionSettings configReader
    let (publishApp, triggerApp) =
        let evt = Event<_>() in (evt.Publish, evt.Trigger)
    let (publishLog, triggerLog) =
        let evt = Event<_>() in (evt.Publish, evt.Trigger)
    let getSessionId, setSessionId = sessionState ()
    let appHandle = applicationHandle setSessionId
    publishApp.Add logAppMsg
    publishApp.Add appHandle
    publishLog.Add logFixMsg
    let socket = new ThreadedSocketAcceptor(
                            app triggerApp, 
                            MemoryStoreFactory (),
                            getSettings (), 
                            logFactory triggerLog)
    let stopPricing () = initPricing getSessionId
    let stop () = stopPricing(); socket.Stop(true); socket.Dispose()
    socket.Start, stop
