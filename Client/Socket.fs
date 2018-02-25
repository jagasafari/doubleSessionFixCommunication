module Socket 

open System
open System.IO
open QuickFix
open QuickFix.Transport
open Client.Model
open Common.Common

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
            } }

let createSocket configPath (logFixMsg, logAppMsg) =
    let getSettings () =
        let config = configPath |> File.ReadAllText
        use configReader = new StringReader(config)
        SessionSettings configReader
    let (publishApp, triggerApp) =
        let evt = Event<_>() in (evt.Publish, evt.Trigger)
    let (publishLog, triggerLog) =
        let evt = Event<_>() in (evt.Publish, evt.Trigger)
    publishApp.Add logAppMsg
    publishLog.Add logFixMsg
    let socket = new SocketInitiator(
                            app triggerApp, 
                            MemoryStoreFactory(), 
                            getSettings (), 
                            logFactory triggerLog)
    let stop () = socket.Stop(true); socket.Dispose()
    socket.Start, stop
