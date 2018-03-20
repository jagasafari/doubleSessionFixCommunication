module Program 

open Common.Common
open Configuration
open Client.Model

let [<EntryPoint>] main _ = 
    configureLog4Net ()
    let logger = log4net.LogManager.GetLogger typeof<Connection> 
    let config = appConfig.Value
    let start, stop = Compose.getApi config logger
    start (); watchToExit (); stop (); 0
