module Program 

open System
open QuickFix
open QuickFix.Transport
open log4net
open Microsoft.Extensions.Configuration
open Common.Common
open Configuration
open DataTypes

let create app factory settings logger =
    let socket = new SocketInitiator(app, factory, settings, logger)
    let stop () = socket.Stop (true)
    socket.Start, stop, socket.Dispose

type internal FixConnection = interface end

let [<EntryPoint>] main _ = 
    let configPath = "fix.cfg"
    
    configureLog4Net ()
    let logger = LogManager.GetLogger typeof<FixConnection>
    let start, stop, clean = createSocket logger configPath (create (App ())) 
    start (); watchToExit (); stop (); clean (); 0
