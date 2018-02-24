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
let ``sig`` () =
    writeType typeof<MDEntrySize>

[<Fact>]
let ``create logReact: log file -> logged once`` () =
    log.Value |> ignore
    log.Value |> ignore

let groupTestData =
    seq {
        yield (0.0, Bid, '0')
        yield (1000.0, Bid, '0')
        yield (1000.0, Ask, '1')
    } |> cast3TestData

[<Theory; MemberData("groupTestData")>]
let ``price group`` price side expectedSide =
    let gr = Prices.group price side
    gr.MDEntrySize.getValue() =! 1000000.0m
    gr.QuoteCondition.getValue() =! "A"
    gr.MDEntryPx.getValue() =! price
    gr.MDEntryType.getValue() =! expectedSide
