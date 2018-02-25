module Tests

open System
open Xunit
open Common.TypeLookUp
open Common.TestUtil
open QuickFix.FIX42
open QuickFix.Fields
open Swensen.Unquote
open Prices
open Log
open Server.Model

[<Fact>]
[<Trait("Category", "Integration")>]
let ``sig`` () =
    writeType typeof<QuickFix.Message>

let logReactTestData =
    [
    (OnCreate null, "OnCreate|SessionId=<null>")
    (OnLogon, "OnLogon")
    (OnLogout, "OnLogout")
    (ToAdmin null, "ToAdmin|Message=")
    (FromAdmin null, "FromAdmin|Message=")
    (ToApp null, "ToApp|Message=")
    (FromApp null, "FromApp|Message=")
    ]
let logFixMsgTestData =
    [
    (OnEvent "abc", "Event|event=abc")
    (OnOutgoing "8=FIX.4.4", "out|8=FIX.4.4")
    (OnIncoming "8=abc\u00015=d", "in|8=abc|5=d")
    ] |> cast2TestData

[<Theory; MemberData("logFixMsgTestData")>]
[<Trait("Category", "Integration")>]
let ``logReact: cases`` msg expected =
    let log, getResult = testCmd ()
    quickFixLogMsgHandle log msg
    getResult () =! expected

let ``create logReact: log file -> logged once`` () =
    log.Value |> ignore
    log.Value |> ignore

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
