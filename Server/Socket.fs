module Socket 

open System 
open System.IO
open QuickFix
open Server.Model

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
                member this.OnEvent(e) = LogEvent e |> react
                member this.OnOutgoing(msg) = LogOutgoing msg |> react
                member this.OnIncoming(msg) = LogIncoming msg |> react
            } 
    }

let createSocket configPath log =
    let getSettings () =
        let config = configPath |> File.ReadAllText
        use configReader = new StringReader(config)
        SessionSettings configReader
    let app = App ()
    let logger = logFactory log
    let factory = MemoryStoreFactory()
    let settings = getSettings ()
    let socket = new ThreadedSocketAcceptor(
                            app, factory, settings, logger)
    let stop () = socket.Stop(true) 
    socket.Start, stop, socket.Dispose
