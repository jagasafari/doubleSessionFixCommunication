module Tests

open System
open Xunit
open Server
open log4net
open System.IO
open SignatureUtil
open System.Reflection
open QuickFix
    
[<Fact>]
let ``sig`` () =
    writeTypeMembers typeof<SessionID>

[<Fact>]
[<Trait("Category", "Integration")>]
let ``log4net: log to file`` () =
    let fi = FileInfo "log4net.config"
    let logRepository = 
            LogManager.GetRepository(Assembly.GetEntryAssembly())
    Config.XmlConfigurator.Configure(logRepository, fi) |> ignore

[<Fact>]
[<Trait("Category", "Integration")>]
let ``create empty test app`` () =
    let configPath = "fix.cfg"
    let start, stop = createSocket configPath
    start ()
    Threading.Thread.Sleep 1000
    stop ()
