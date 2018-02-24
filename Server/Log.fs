module Log

open Server.Model
open Common.Common

let logReact info = function
    | FixEvent x -> x |> sprintf "FixEvent|event=%s" |> info
    | FixMsgOutgoing x -> x |> parseFixMsg |> sprintf "out|%s" |> info
    | FixMsgIncoming x -> x |> parseFixMsg |> sprintf "in|%s" |> info

let log = 
    lazy
        configureLog4Net ()
        let logger = 
            log4net.LogManager.GetLogger typeof<Connection> 
        logger.Info "logger created"
        logger.Info |> logReact

