module Program 

open System 
open QuickFix
open DataTypes
open Common.Common

let create app factory settings logger =
    let socket = new ThreadedSocketAcceptor(app, factory, settings, logger)
    let stop () = socket.Stop(true); socket.Dispose()
    socket.Start, stop

let [<EntryPoint>] main _ =
    let configPath = "fix.cfg"
    let start, stop = createSocket (create (App ())) configPath
    start (); watchToExit (); stop (); 0
