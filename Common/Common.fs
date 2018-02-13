module Common.Common

open System
open System.IO
open QuickFix
open QuickFix.Transport

let rec watchToExit () =
    let key = Console.ReadKey ()
    match key.Key with
    | ConsoleKey.D -> () | _ -> watchToExit ()

type Log4NetFactory () =
    interface ILogFactory with
        member this.Create(sessionId) =
            let print msg = printfn "%A" msg
            print sessionId
            { new ILog with
                member this.Clear() = ()
                member this.Dispose() = ()
                member this.OnEvent(e) = e |> sprintf "event: %A" |> print
                member this.OnOutgoing(msg) = msg |> sprintf "out: %A" |> print
                member this.OnIncoming(msg) = msg |> sprintf "in: %A" |> print }

let createSocket create (configPath: string) =
    let config = configPath |> File.ReadAllText
    let configReader = new StringReader(config)
    let settings = SessionSettings configReader
    let factory = MemoryStoreFactory()
    let logger = Log4NetFactory()
    let start, stopSocket = create factory settings logger
    let stop () = configReader.Dispose(); stopSocket ()
    start, stop
