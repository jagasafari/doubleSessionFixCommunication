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

let applicationMsgLoggingHandle info msg =
    let name = getUnionCaseName<ApplicationMsg> msg
    let logMsg = parseFixMsg >> sprintf "%s|Message=%s" name >> info
    let logSessionId = sprintf "%s|SessionId=%A" name >> info
    match msg with
    | OnCreate x | OnLogon x | OnLogout x -> logSessionId x
    | ToAdmin x | FromAdmin x | ToApp x | FromApp x -> logMsg x

let log = 
    lazy
        configureLog4Net ()
        let logger = 
            log4net.LogManager.GetLogger typeof<Connection> 
        logger.Info "logger created"
        let logFixMsg = quickFixLogMsgHandle logger.Debug logger.Info
        let logAppMsg = logger.Info |> applicationMsgLoggingHandle
        (logFixMsg, logAppMsg)
