module Log

open Server.Model
open Common.Common

let logReact info = function
    | LogEvent x -> x |> sprintf "event|event=%s" |> info
    | LogOutgoing x -> x |> parseFixMsg |> sprintf "out|%s" |> info
    | LogIncoming x -> x |> parseFixMsg |> sprintf "in|%s" |> info

let log = 
    lazy
        configureLog4Net ()
        let logger = 
            log4net.LogManager.GetLogger typeof<FixServerConnection> 
        logger.Info "logger created"
        logger.Info |> logReact

