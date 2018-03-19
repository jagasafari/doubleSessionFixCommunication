module Compose

open System
open Client.Model

let getLog () = 
    log4net.LogManager.GetLogger typeof<Connection>

let getApi (config: AppConfig) (log: log4net.ILog) =
    let log: log4net.ILog = getLog ()
    let (publishApp, triggerApp) =
        let evt = Event<_>() in (evt.Publish, evt.Trigger)
    let (publishLog, triggerLog) =
        let evt = Event<_>() in (evt.Publish, evt.Trigger)
    publishApp.Add (SocketLog.logAppMsg log.Info)
    let decryptPwd = Util.decrypt config.Password
    let fillLogonMsg = 
        FixServerCommunication.fillLogonMsg decryptPwd
    (config.HeartbeatFrequency, config.User)
    |> fillLogonMsg
    |> FixServerCommunication.appHandle
    |> publishApp.Add
    SocketLog.logQuickFixMsg log.Debug log.Info
    |> publishLog.Add 
    let startSocket, stopSocket =
        Socket.createSocket 
            config.QuickFixConfigFile triggerApp triggerLog
    let start () = startSocket ()
    let stop () = stopSocket ()
    start, stop
