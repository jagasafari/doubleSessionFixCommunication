module Log

open System
open Client.Model
open Common.Common

let isPricingMsg = function 
    | (msg: string) when msg.Contains("35=W") -> true | _ -> false

let logReact (debug, info) = function
    | FixEvent x -> x |> sprintf "FixEvent|event=%s" |> info
    | FixMsgOutgoing x -> x |> parseFixMsg |> sprintf "out|%s" |> info
    | FixMsgIncoming x -> 
            parseFixMsg x 
            |> sprintf "in|%s" 
            |> if isPricingMsg x then debug else info

let log = 
    lazy
        configureLog4Net ()
        let logger = 
            log4net.LogManager.GetLogger typeof<Connection> 
        logger.Info "logger created"
        (logger.Debug, logger.Info) |> logReact

