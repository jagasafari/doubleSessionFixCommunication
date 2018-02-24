module Socket 

open System
open System.IO
open QuickFix
open QuickFix.Transport
open log4net
open DataTypes

type internal FixConnection = interface end

let createSocket configPath =
    let getSettings () =
        let config = configPath |> File.ReadAllText
        use configReader = new StringReader(config)
        SessionSettings configReader
    let log = LogManager.GetLogger 
                            typeof<FixConnection>
    let app = App ()
    let logger = PricingLogFactory log
    let factory = MemoryStoreFactory()
    let settings = getSettings ()
    let socket = new ThreadedSocketAcceptor(
                            app, factory, settings, logger)
    let stop () = socket.Stop(true) 
    socket.Start, stop, socket.Dispose
