module Tests

open Xunit
open Swensen.Unquote
open QuickFix
open Configuration
open Client.Model
open Log
open Common.TestUtil

let logFixMsgTestData =
    [
    (OnEvent "abc", "Event|event=abc", "")
    (OnOutgoing "8=FIX.4.4", "out|8=FIX.4.4", "")
    (OnIncoming "8=abc\u00015=d", "in|8=abc|5=d", "")
    (OnIncoming "8=abc\u00015=d\u000135=W", "", "in|8=abc|5=d|35=W")
    ] |> cast3TestData

[<Theory; MemberData("logFixMsgTestData")>]
[<Trait("Category", "Integration")>]
let ``logReact: cases`` msg expectedInfo expectedDebug =
    let logDebug, getDebug = testCmd ()
    let logInfo, getInfo = testCmd ()
    logQuickFixMsg logDebug logInfo msg
    getDebug () =! expectedDebug
    getInfo () =! expectedInfo

[<Fact>]
let ``getConfig: `` () =
    let cfg: AppConfig = appConfig.Value
    cfg.QuickFixConfigFile =! "fix.cfg"
    cfg.HeartbeatFrequency =! 60
    cfg.User =! "user"
