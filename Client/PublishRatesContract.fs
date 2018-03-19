module PublishRatesContract

open System
open Grpc.Core
open MBrace.FsPickler

type Rate =
    {
        Symbol: string
        Ask: decimal
        Bid: decimal
        SourceSendingTime: DateTime
    }

type RatesStreamingResponse = | PricingStopped

let createMarshaller () = 
    let bs = FsPickler.CreateBinarySerializer ()
    Marshallers.Create(
        Func<_,_>(bs.Pickle), 
        Func<_,_>(bs.UnPickle))

let ratesStreamingContract = 
    Method<Rate, RatesStreamingResponse> (
        MethodType.ClientStreaming,
        "Pricing",
        "StreamRates",
        createMarshaller (),
        createMarshaller ())
