module Socket 

open System
open System.IO
open QuickFix
open QuickFix.Transport
open Client.Model
open Common.Common

type App () =
    interface IApplication with
        member this.OnCreate(sessionId) = ()
        member this.FromAdmin(message, sessionId) = ()    
        member this.ToAdmin(message, sessionId) = ()    
        member this.OnLogon(sessionId) = ()    
        member this.OnLogout(sessionId) = ()    
        member this.FromApp(message, sessionId) = ()    
        member this.ToApp(message, sessionId) = ()    

let logFactory react =
    { new ILogFactory with
        member this.Create(sessionId) =
            { new ILog with
                member this.Clear() = ()
                member this.Dispose() = ()
                member this.OnEvent(e) = FixEvent e |> react 
                member this.OnOutgoing(msg) = FixMsgOutgoing msg |> react
                member this.OnIncoming(msg) = FixMsgIncoming msg |> react
            } }

let createSocket configPath log =
    let getSettings () =
        let config = configPath |> File.ReadAllText
        use configReader = new StringReader(config)
        SessionSettings configReader
    let socket = new ThreadedSocketAcceptor(
                            App (), 
                            MemoryStoreFactory(), 
                            getSettings (), 
                            logFactory log)
    let stop () = socket.Stop(true); socket.Dispose()
    socket.Start, stop
