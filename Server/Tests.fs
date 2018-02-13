module Tests

open System
open Xunit
open log4net
open System.IO
open Common.SignatureUtil
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
