module Program 

open System 
open QuickFix
open Common.Common
open Configuration
open Socket
open Log

let [<EntryPoint>] main _ =
    let config = appConfig.Value
    let start, stop, clear = 
        createSocket config.QuickFixConfigFile log.Value
    start (); watchToExit (); stop (); clear(); 0
