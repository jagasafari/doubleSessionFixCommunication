module Program 

open System 
open QuickFix
open Common.Common
open Configuration
open Socket
open log4net

let [<EntryPoint>] main _ =
    configureLog4Net ()
    let config = appConfig.Value
    let start, stop, clear = createSocket config.QuickFixConfigFile
    start (); watchToExit (); stop (); clear(); 0
