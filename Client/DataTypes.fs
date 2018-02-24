module DataTypes

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

type PricingLogFactory (log: log4net.ILog) =
    let isPricingMsg = function 
        | (msg: string) when msg.Contains("35=W") -> true | _ -> false
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
                                let logMsg = 
                                    if isPricingMsg msg then log.Debug else log.Info
                                msg |> parseFixMsg |> sprintf "in|%s" |> logMsg }

type TradingLogFactory (log: log4net.ILog) =
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
