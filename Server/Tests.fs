module Tests

open System
open System.Threading
open Xunit
open Common.TypeLookUp
open Common.TestUtil
open QuickFix.FIX42
open QuickFix.Fields
open Swensen.Unquote
open Prices
open Socket
open Log
open Server.Model

let logError _ = ()

[<Fact>]
let ``timer started`` () =
    let add, get = mock ()
    let callback () = add "1"
    let stop = timer logError 200 callback
    get () =! []
    Thread.Sleep 210
    get () =! ["1"]
    stop ()

[<Fact>]
let ``timer callback throws`` () =
    let add, get = mock ()
    let callback () = failwith "1"
    let stop = timer logError 200 callback
    get () =! []
    Thread.Sleep 210
    get () =! []
    stop ()

[<Fact>]
let ``session state:`` () =
    let get, set = state ()
    get () =! None
    set (Some 5)
    get () =! Some 5
    set None

[<Fact>]
[<Trait("Category", "Integration")>]
let ``sig`` () =
    writeType typeof<QuickFix.Message>

[<Fact>]
let ``appHandle: setSesstion`` () =
    let add, get = mock ()
    let handle = appHandle add
    handle (OnLogon null)
    get () =! [Some null]
    handle (OnLogout null)
    get () =! [Some null; None]
    handle (OnCreate null)
    get () =! [Some null; None]

let appMsgTestData =
    [
    (OnCreate null, "OnCreate|SessionId=<null>")
    (OnLogon null, "OnLogon|SessionId=<null>")
    (OnLogout null, "OnLogout|SessionId=<null>")
    (ToAdmin null, "ToAdmin|Message=")
    (FromAdmin null, "FromAdmin|Message=")
    (ToApp null, "ToApp|Message=")
    (FromApp null, "FromApp|Message=")
    ] |> cast2TestData

[<Theory; MemberData("appMsgTestData")>]
[<Trait("Category", "Integration")>]
let ``logappMsg: cases`` msg expected =
    let log, get = mock ()
    logAppMsg log msg
    get () =! [expected]

let logFixMsgTestData =
    [
    (OnEvent "abc", "Event|event=abc")
    (OnOutgoing "8=FIX.4.4", "out|8=FIX.4.4")
    (OnIncoming "8=abc\u00015=d", "in|8=abc|5=d")
    ] |> cast2TestData

[<Theory; MemberData("logFixMsgTestData")>]
[<Trait("Category", "Integration")>]
let ``logReact: cases`` msg expected =
    let log, get = mock ()
    logQuickFixMsg log msg
    get () =! [expected]

let groupTestData =
    [
    (0.0, Bid, '0')
    (1000.0, Bid, '0')
    (1000.0, Ask, '1')
    ] |> cast3TestData

[<Theory; MemberData("groupTestData")>]
let ``price group`` price side expectedSide =
    let gr = Prices.group price side
    gr.MDEntrySize.getValue() =! 1000000.0m
    gr.QuoteCondition.getValue() =! "A"
    gr.MDEntryPx.getValue() =! price
    gr.MDEntryType.getValue() =! expectedSide
