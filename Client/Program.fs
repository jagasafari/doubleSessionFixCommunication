module Program 

open System
open Common.Common
open Configuration
open Socket

let [<EntryPoint>] main _ = 
    configureLog4Net ()
    let config = appConfig.Value
    let start, stop, clean = createSocket config.QuickFixConfigFile
    start (); watchToExit (); stop (); clean (); 0
