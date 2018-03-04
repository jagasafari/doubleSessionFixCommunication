module Compose

open System
open Client.Model

let getApi (config: AppConfig) (log: log4net.ILog) =
    let log: log4net.ILog = Log.getLog ()
    let (publishApp, triggerApp) =
        let evt = Event<_>() in (evt.Publish, evt.Trigger)
    let (publishLog, triggerLog) =
        let evt = Event<_>() in (evt.Publish, evt.Trigger)
    publishApp.Add (Log.logAppMsg log.Info)
    let decryptPwd = Util.decrypt config.Password
    let fillLogonMsg = FixServerCommunication.fillLogonMsg decryptPwd
    (config.HeartbeatFrequency, config.User)
    |> fillLogonMsg
    |> FixServerCommunication.appHandle
    |> publishApp.Add
    publishLog.Add (Log.logQuickFixMsg log.Debug log.Info)
    Socket.createSocket config.QuickFixConfigFile triggerApp triggerLog
