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
            log4net.LogManager.GetRepository(
                                Assembly.GetEntryAssembly())
    log4net.Config.XmlConfigurator.Configure(logRepository, fi) |> ignore

let parseFixMsg (msg: obj) = 
    match msg with
    | null -> String.Empty
    | :? string as x -> x.Replace('\u0001', '|')
    | :? QuickFix.Message as x -> String.Empty
    | x -> x |> sprintf "%A"
