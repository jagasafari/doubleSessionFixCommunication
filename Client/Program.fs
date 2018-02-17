module Program 

open System
open DataTypes
open QuickFix
open QuickFix.Transport
open Common.Common
open log4net

let create app factory settings logger =
    let socket = new SocketInitiator(app, factory, settings, logger)
    let stop () = socket.Stop (true)
    socket.Start, stop, socket.Dispose

type internal FixConnection = interface end
let [<EntryPoint>] main _ = 
    configureLog4Net ()
    let logger = LogManager.GetLogger typeof<FixConnection>
    let configPath = "fix.cfg"
    let start, stop, clean = createSocket logger configPath (create (App ())) 
    start (); watchToExit (); stop (); clean (); 0
