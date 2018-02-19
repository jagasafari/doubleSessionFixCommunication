module Tests

open Xunit
open Swensen.Unquote
open Microsoft.Extensions.Configuration
open DataTypes
open Configuration

[<Fact>]
let ``getConfig: `` () =
    let cfg: AppConfig = appConfig.Value
    cfg.ReactIntervalStart =! 1
    cfg.ReactIntervalRun =! 1
    cfg.ReactIntervalStop =! 30
