module Compose

open System
open Client.Model
open Log
open Socket

let getApi (config: AppConfig) (log: log4net.ILog) =
    let (publishApp, triggerApp) =
        let evt = Event<_>() in (evt.Publish, evt.Trigger)
    let (publishLog, triggerLog) =
        let evt = Event<_>() in (evt.Publish, evt.Trigger)
    publishApp.Add (logAppMsg log.Info)
    (config.HeartbeatFrequency, config.User, config.Password)
    |> updateLogonMsg
    |> appHandle
    |> publishApp.Add
    publishLog.Add (logQuickFixMsg log.Debug log.Info)
    createSocket config.QuickFixConfigFile triggerApp triggerLog
