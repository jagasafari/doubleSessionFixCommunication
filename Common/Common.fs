module Common.Common

open System
open System.IO
open QuickFix
open QuickFix.Transport
open System.Reflection

let rec watchToExit () =
    let key = Console.ReadKey ()
    match key.Key with
    | ConsoleKey.D -> () | _ -> watchToExit ()

let configureLog4Net () =
    let fi = FileInfo "log4net.config"
    let logRepository = 
            log4net.LogManager.GetRepository(Assembly.GetEntryAssembly())
    log4net.Config.XmlConfigurator.Configure(logRepository, fi) |> ignore

let parseFixMsg (msg: string) = 
    let posOfSeperator = 9
    msg.Replace(msg.[posOfSeperator], '|')

type MyLogFactory (log: log4net.ILog) =
    let isPricingMsg = function 
        | (msg: string) when msg.Contains("35=W") -> true | _ -> false
    interface ILogFactory with
        member this.Create(sessionId) =
            { new ILog with
                member this.Clear() = ()
                member this.Dispose() = ()
                member this.OnEvent(e) = 
                                e |> sprintf "event|event=%s" |> log.Info
                member this.OnOutgoing(msg) = 
                                msg |> parseFixMsg |> sprintf "out|%s" |> log.Info
                member this.OnIncoming(msg) = 
                                let logMsg = 
                                    if isPricingMsg msg then log.Debug else log.Info
                                msg |> parseFixMsg |> sprintf "in|%s" |> logMsg }

let createSocket log (configPath: string) create =
    let getSettings () =
        let config = configPath |> File.ReadAllText
        use configReader = new StringReader(config)
        SessionSettings configReader
    let factory = MemoryStoreFactory()
    let settings = getSettings ()
    let logger = MyLogFactory log
    create factory settings logger
