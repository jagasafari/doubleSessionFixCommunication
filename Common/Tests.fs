module Tests

open System
open Xunit
open Swensen.Unquote
open log4net
open System.IO
open System.Reflection
open Common.TypeLookUp
open Common.Common
    
type Letter = | A | B

[<Fact>]
let ``name of union case`` () =
    let example = B
    getUnionCaseName<Letter> example =! "B"

[<Fact>]
[<Trait("Category", "Integration")>]
let ``sig`` () = writeType typeof<LogManager>

type internal FixConnection = interface end

[<Fact>]
[<Trait("Category", "Integration")>]
let ``log4net: log to file`` () =
    let fi = FileInfo "log4net.config"
    let logRepository = 
            LogManager.GetRepository(Assembly.GetEntryAssembly())
    Config.XmlConfigurator.Configure(logRepository, fi) |> ignore
    let logger = LogManager.GetLogger typeof<FixConnection>
    logger.Info "########################################"
