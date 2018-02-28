module Tests

open Xunit
open Swensen.Unquote
open Configuration
open Log
open Client.Model
open Common.TestUtil
open System.Threading

let timer logError interval callback =
    let c _ = try callback () with | e -> logError e
    let tc = TimerCallback c
    let t = new Timer(tc, null, interval, Timeout.Infinite)
    fun () -> t.Dispose ()

let startSubscribing callback () =
    let log = getLog ()
    timer log.Error 500 callback

let handleSubscriptions refresh = ()

let refreshCache pull =
    let mutable cache: string list option = None
    let refresh () = [], []
    fun () -> refresh ()

let subscriptionTestData =
    [
    ([1;4;6;0],[1;4;6;0],[])
    ] |> cast3TestData

[<Theory; MemberData("subscriptionTestData")>]
let ``subscriptions cache`` current request reject =
    let pull () = current
    refreshCache pull () =! (request, reject)
    
    ()

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
