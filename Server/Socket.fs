module Socket 

open System 
open System.IO
open QuickFix
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

type MyLogFactory (log: log4net.ILog) =
    interface ILogFactory with
        member this.Create(sessionId) =
            { new ILog with
                member this.Clear() = ()
                member this.Dispose() = ()
                member this.OnEvent(e) = 
                                e |> sprintf "event|event=%s" |> log.Info
                member this.OnOutgoing(msg) = 
                                msg |> parseFixMsg |> sprintf "out|%s" |> log.Info
                member this.OnIncoming(msg) = 
                                msg |> parseFixMsg |> sprintf "in|%s" |> log.Info }

type FixServerConnection = interface end

let createSocket configPath =
    let getSettings () =
        let config = configPath |> File.ReadAllText
        use configReader = new StringReader(config)
        SessionSettings configReader
    let log = log4net.LogManager.GetLogger 
                            typeof<FixServerConnection>
    let app = App ()
    let logger = MyLogFactory log
    let factory = MemoryStoreFactory()
    let settings = getSettings ()
    let socket = new ThreadedSocketAcceptor(
                            app, factory, settings, logger)
    let stop () = socket.Stop(true) 
    socket.Start, stop, socket.Dispose
