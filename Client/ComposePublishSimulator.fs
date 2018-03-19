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
    let startChannel () = 
        match channelHandler CreateChannel with
        | CallInvoker invoker -> invoker
        | x -> 
            logChannel x 
            "FatalError|Message=PublishRatesChannelNotCreated"
            |> failwith
    let start () = 
        let invoker = startChannel ()
        let push = 
            getRatesStreaming invoker config.RatePushingDeadline
        ()
    let stop () = channelHandler ShutDown |> logChannel
    start, stop
