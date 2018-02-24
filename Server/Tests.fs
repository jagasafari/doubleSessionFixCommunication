module Tests

open System
open Xunit
open Common.SignatureUtil
open QuickFix.FIX42
open QuickFix.Fields
open Swensen.Unquote
open Prices
    
[<Fact>]
let ``sig`` () =
    writeTypeMembers typeof<MDEntrySize>

let cast3TestData =
    let castTuple (x, y, z) = [|x:>obj; y:>obj; z:>obj|]
    Seq.map castTuple

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
