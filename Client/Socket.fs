module Socket 

open System
open System.IO
open QuickFix
open QuickFix.Fields
open QuickFix.Transport
open Client.Model
open Common.Common

let app react =
    { new IApplication with
        member __.OnCreate(sessionId) = OnCreate sessionId |> react
        member __.FromAdmin(msg, _) = FromAdmin msg |> react
        member __.ToAdmin(msg, _) = ToAdmin msg |> react
        member __.OnLogon(sessionId) = OnLogon sessionId |> react    
        member __.OnLogout(sessionId) = OnLogout sessionId |> react    
        member __.FromApp(msg, _) = FromApp msg |> react
        member __.ToApp(msg, _) = ToApp msg |> react
    }

let logFactory react =
    { new ILogFactory with
        member __.Create(sessionId) =
            { new ILog with
                member __.Clear() = ()
                member __.Dispose() = ()
                member __.OnEvent(e) = OnEvent e |> react 
                member __.OnOutgoing(msg) = OnOutgoing msg |> react
                member __.OnIncoming(msg) = OnIncoming msg |> react
            } 
    }

let createSocket configPath triggerApp triggerLog =
    let getSettings () =
        let config = configPath |> File.ReadAllText
        use configReader = new StringReader(config)
        SessionSettings configReader
    let socket = new SocketInitiator(
                            app triggerApp, 
                            MemoryStoreFactory(), 
                            getSettings (), 
                            logFactory triggerLog)
    let stop () = socket.Stop(true); socket.Dispose()
    socket.Start, stop
