module Prices

open System
open System.Threading
open QuickFix
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

let pairFullRefresh pair groups =
    let pairRefresh = MarketDataSnapshotFullRefresh ()
    pairRefresh.Symbol <- Symbol pair
    pairRefresh.MDReqID <- MDReqID pair
    let numGroups = groups |> List.length
    pairRefresh.NoMDEntries <- NoMDEntries numGroups
    groups |> List.iter pairRefresh.AddGroup
    pairRefresh

let sendPairFullRefresh getSessionId () =
    match getSessionId () with
    | Some id ->
        [ group 4.21m Bid; group 1.11m Ask ]
        |> pairFullRefresh "EUR/PLN"
        |> Session.LookupSession(id).Send
        |> ignore
    | None -> ()

let timer interval callback =
    let c = TimerCallback(fun _ -> callback())
    let t = new Timer(c, null, interval, Timeout.Infinite)
    fun () -> t.Dispose ()

let initPricing getSessionId = 
    getSessionId |> sendPairFullRefresh |> timer 200
