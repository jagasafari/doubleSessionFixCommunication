module Tests

open Xunit
open Swensen.Unquote
open Configuration
open Log
open Client.Model
open Common.TestUtil

let logReactTestData =
    [
    (OnEvent "abc", "Event|event=abc", "")
    (OnOutgoing "8=FIX.4.4", "out|8=FIX.4.4", "")
    (OnIncoming "8=abc\u00015=d", "in|8=abc|5=d", "")
    (OnIncoming "8=abc\u00015=d\u000135=W", "", "in|8=abc|5=d|35=W")
    ] |> cast3TestData

[<Theory; MemberData("logReactTestData")>]
[<Trait("Category", "Integration")>]
let ``logReact: cases`` msg expectedInfo expectedDebug =
    let logDebug, getResultDebug = testCmd ()
    let logInfo, getResultInfo = testCmd ()
    quickFixLogMsgHandle logDebug logInfo msg
    getResultDebug () =! expectedDebug
    getResultInfo () =! expectedInfo

[<Fact>]
let ``getConfig: `` () =
    let cfg: AppConfig = appConfig.Value
    cfg.QuickFixConfigFile =! "fix.cfg"
