module DataTypes

open QuickFix

type Log4NetFactory () =
    interface ILogFactory with
        member this.Create(sessionId) =
            let print msg = printfn "%A" msg
            print sessionId
            { new ILog with
                member this.Clear() = ()
                member this.Dispose() = ()
                member this.OnEvent(e) = print e
                member this.OnOutgoing(msg) = print msg
                member this.OnIncoming(msg) = print msg }

type ServerApp () =
    interface IApplication with
        member this.OnCreate(sessionId) = ()
        member this.FromAdmin(message, sessionId) = ()    
        member this.ToAdmin(message, sessionId) = ()    
        member this.OnLogon(sessionId) = ()    
        member this.OnLogout(sessionId) = ()    
        member this.FromApp(message, sessionId) = ()    
        member this.ToApp(message, sessionId) = ()    
