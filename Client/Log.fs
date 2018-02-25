module Log

open System
open Client.Model
open Common.Common

let isPricingMsg = function 
    | (msg: string) when msg.Contains("35=W") -> true | _ -> false

let quickFixLogMsgHandle debug info = function
    | OnEvent x -> x |> sprintf "Event|event=%s" |> info
    | OnOutgoing x -> x |> parseFixMsg |> sprintf "out|%s" |> info
    | OnIncoming x -> 
            parseFixMsg x 
            |> sprintf "in|%s" 
            |> if isPricingMsg x then debug else info

let applicationMsgLoggingHandle info = function
    | OnCreate x -> x |> sprintf "OnCreate|SessionId=%A" |> info
    | OnLogon -> info "OnLogon"
    | OnLogout -> info "OnLogout"
    | ToAdmin x -> 
        x |> parseFixMsg |> sprintf "ToAdmin|Message=%s" |> info
    | FromAdmin x -> 
        x |> parseFixMsg |> sprintf "FromAdmin|Message=%s" |> info
    | ToApp x -> 
        x |> parseFixMsg |> sprintf "ToApp|Message=%s" |> info
    | FromApp x -> 
        x |> parseFixMsg |> sprintf "FromApp|Message=%s" |> info

let log = 
    lazy
        configureLog4Net ()
        let logger = 
            log4net.LogManager.GetLogger typeof<Connection> 
        logger.Info "logger created"
        let logFixMsg = quickFixLogMsgHandle logger.Debug logger.Info
        let logAppMsg = logger.Info |> applicationMsgLoggingHandle
        (logFixMsg, logAppMsg)
