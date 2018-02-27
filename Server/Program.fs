module Program 

open Common.Common
open Server.Model
open Configuration
open Compose

let [<EntryPoint>] main _ =
    configureLog4Net ()
    let logger = 
        log4net.LogManager.GetLogger typeof<Connection> 
    let config = appConfig.Value
    let start, stop = getApi config logger
    start (); watchToExit (); stop (); 0
