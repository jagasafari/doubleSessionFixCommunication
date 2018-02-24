module Tests

open Xunit
open Swensen.Unquote
open DataTypes
open Configuration

[<Fact>]
let ``getConfig: `` () =
    let cfg: AppConfig = appConfig.Value
    cfg.QuickFixConfigFile =! "fix.cfg"
