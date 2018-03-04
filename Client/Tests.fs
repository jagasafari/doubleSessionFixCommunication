module Tests

open Xunit
open Swensen.Unquote
open QuickFix
open Configuration
open CurrencyPairSubscriptions
open Client.Model
open Log
open Common.TestUtil
open System.Threading

let mutateSubsTestData =
    [
    (Reset, "reset")
    (SetCache (["2";"3"]|>Set.ofList), "set [\"2\"; \"3\"]")
    (SetCache Set.empty, "set []")
    (Remove "5", "remove|\"5\"")
    ] |> cast2TestData

[<Theory; MemberData("mutateSubsTestData")>]
let ``mutateSubscriptionCache: cases`` msg expected = 
    let set x = sprintf "%A" x
    let reset () = "reset"
    let remove x = sprintf "remove|%A" x
    mutateSubscriptionCache set reset remove msg =! expected

[<Fact>]
let ``subscriptionCache: state changes`` () =
    let get, mutate = subscriptionCache ()
    let test x = 
        get () =! (x |> Set.ofList)
    test []
    SetCache (["1";"4"]|>Set.ofList) |> mutate
    test ["1";"4"]
    Remove "4" |> mutate
    test ["1"]
    Reset |> mutate
    test []
    
    

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
