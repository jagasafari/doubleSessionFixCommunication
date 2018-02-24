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
    writeType typeof<MDEntrySize>

let logReactTestData =
    [
    (FixEvent "abc", "FixEvent|event=abc")
    (FixMsgOutgoing "8=FIX.4.4", "out|8=FIX.4.4")
    (FixMsgIncoming "8=abc\u00015=d", "in|8=abc|5=d")
    ] |> cast2TestData

[<Theory; MemberData("logReactTestData")>]
[<Trait("Category", "Integration")>]
let ``logReact: cases`` msg expected =
    let log, getResult = testCmd ()
    logReact log msg
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
