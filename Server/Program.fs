module Program 

open System 
open QuickFix
open DataTypes
open Common.Common
open log4net

let create app factory settings logger =
    let socket = new ThreadedSocketAcceptor(app, factory, settings, logger)
    let stop () = socket.Stop(true) 
    socket.Start, stop, socket.Dispose

type FixServerConnection = interface end

let [<EntryPoint>] main _ =
    configureLog4Net ()
    let configPath = "fix.cfg"
    let log = LogManager.GetLogger typeof<FixServerConnection>
    let start, stop, clear = createSocket log configPath (create (App ())) 
    start (); watchToExit (); stop (); clear(); 0
