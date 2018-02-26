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

let send sessionId msg =
    Session.SendToTarget(msg, sessionId) |> ignore

let sendPairFullRefresh getSessionId () =
    match getSessionId () with
    | Some id ->
        [ group 4.21m Bid; group 1.11m Ask ]
        |> pairFullRefresh "EUR/PLN"
        |> send id
        |> ignore
    | None -> ()

let timer logError interval callback =
    let c _ = try callback () with | e -> logError e
    let tc = TimerCallback c
    let t = new Timer(tc, null, interval, Timeout.Infinite)
    fun () -> t.Dispose ()

let initPricing logError getSessionId = 
    let callback = sendPairFullRefresh getSessionId 
    timer logError 200 callback
