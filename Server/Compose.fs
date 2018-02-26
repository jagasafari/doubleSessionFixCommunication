module Compose

open System
open Server.Model
open Prices
open Socket
open Log

let getApi (config: AppConfig) (log: log4net.ILog) =
    let (publishApp, triggerApp) =
        let evt = Event<_>() in (evt.Publish, evt.Trigger)
    let (publishLog, triggerLog) =
        let evt = Event<_>() in (evt.Publish, evt.Trigger)
    let getSessionId, setSessionId = state ()
    let appHandle = appHandle setSessionId
    publishApp.Add (logAppMsg log.Info)
    publishApp.Add appHandle
    publishLog.Add (logQuickFixMsg log.Info)
    let logErr= logError log.Error
    let stopPricing = initPricing logErr getSessionId
    createSocket 
        config.QuickFixConfigFile stopPricing triggerApp triggerLog
 
    
    
    
