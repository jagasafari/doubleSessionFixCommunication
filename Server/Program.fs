module Program 

open System 
open QuickFix
open DataTypes
open Common.Common
open log4net

let create app logger factory settings =
    let socket = new ThreadedSocketAcceptor(app, factory, settings, logger)
    let stop () = socket.Stop(true) 
    socket.Start, stop, socket.Dispose

type FixServerConnection = interface end

let [<EntryPoint>] main _ =
    configureLog4Net ()
    let configPath = "fix.cfg"
    let log = LogManager.GetLogger typeof<FixServerConnection>
    let create' = create (App ()) (MyLogFactory log)
    let start, stop, clear = createSocket configPath create'
    start (); watchToExit (); stop (); clear(); 0
