module Common.Common

open System
open System.IO
open QuickFix
open QuickFix.Transport
open System.Reflection
open Microsoft.FSharp.Reflection


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
