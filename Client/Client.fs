module Client

open System
open QuickFix
open QuickFix.Transport
open DataTypes
open System.IO

let createSocket (configPath: string) =
    let config = File.ReadAllText configPath
    let configReader = new StringReader(config)
    let settings = SessionSettings configReader
    let factory = MemoryStoreFactory()
    let logger = Log4NetFactory()
    let app = App () 
    let socket = new SocketInitiator(app, factory, settings, logger)
    let start () = socket.Start()
    let stop () = 
        configReader.Dispose()
        socket.Stop(true) 
        socket.Dispose()
    start, stop
