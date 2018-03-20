module ComposePublishSimulator

open System
open Client.Model
open PublishRates
open PublishRatesContract

let mockRate =
    {
    Symbol = "Eur/Pln"
    Ask = 4.342m
    Bid = 1.231m
    SourceSendingTime = DateTime(2018,02,12,11,34,23,899)
    }

let getLog () = 
    log4net.LogManager.GetLogger typeof<Connection>

let getApi (config: AppConfig) (log: log4net.ILog) =
    let log: log4net.ILog = getLog ()
    let timeout = TimeSpan.FromSeconds 10.
    let channelHandler = 
        channel 
            timeout config.PublishRatesHost config.PublishRatesPort
    let logChannel = logChannel log.Info log.Warn log.Error
    let startChannel' = startChannel logChannel channelHandler
    let logPush = logPushRate log.Info log.Error
    let push' = push logPush config.RatePushingDeadline
    let startSimulate, stopSimulate = simulatePush ()
    let start () = 
        let invoker = startChannel' ()
        let pushRate _ = push' invoker mockRate
        startSimulate pushRate
    let stop () = 
        stopSimulate ()
        channelHandler ShutDown |> logChannel
    start, stop
