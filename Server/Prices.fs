module Prices

open System
open QuickFix.FIX42
open QuickFix.Fields
open Server.Model

let emptyGroup () =
    MarketDataSnapshotFullRefresh.NoMDEntriesGroup() 

let entryType = function
     Bid -> MDEntryType.BID | Ask -> MDEntryType.OFFER

let group price side =
    let gr = emptyGroup ()
    gr.MDEntrySize <- MDEntrySize 1000000.m
    gr.QuoteCondition <- QuoteCondition "A"
    gr.MDEntryPx <- MDEntryPx price
    gr.MDEntryType <- side |> entryType |> MDEntryType 
    gr
