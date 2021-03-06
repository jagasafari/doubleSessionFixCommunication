module PublishRatesTests

open Xunit
open System
open Swensen.Unquote
open FSharp.Control.Reactive
open Client.Model
open Common.TestUtil
open PublishRates
open PublishRatesContract
open Grpc.Core
open Common.TypeLookUp
open MBrace.FsPickler

let channelHandler () =
    let timeout = TimeSpan.FromSeconds 5.
    channel timeout "abc" 345

let mockRate =
    {
    Symbol = "Eur/Pln"
    Ask = 4.342m
    Bid = 1.231m
    SourceSendingTime = DateTime(2018,02,12,11,34,23,899)
    }

[<Fact>]
let ``enocde, decode rate`` () =
    let bs = FsPickler.CreateBinarySerializer()
    let ser = bs.Pickle mockRate
    let back = bs.UnPickle ser
    back =! mockRate
    createMarshaller () |> ignore

let createRatesStreamingCall (invoker: DefaultCallInvoker) = 
    invoker.AsyncClientStreamingCall(
        ratesStreamingContract, null, CallOptions())

[<Fact>]
let ``streaming call`` () =
    let handler = channelHandler ()
    let invoker = 
        match handler CreateChannel with 
        | CallInvoker x ->x | _ -> failwith "fails"
    let call: AsyncClientStreamingCall<_,_> = 
        createRatesStreamingCall invoker
    let timeout = TimeSpan.FromMilliseconds 50.
    handler ShutDown
    
type ArroundMocking = 
    | Simple of ChannelResult 
    | NotNiceToTest

let channelCmdDataTests =
    [
    (
        [ShutDown;GetState], 
        [
            Simple NothingToShutDown;
            Simple CanNotRetrieveState
        ]
    )
    (
        [CreateChannel], 
        [NotNiceToTest]
    )
    (
        [CreateChannel;CreateChannel], 
        [NotNiceToTest;Simple CanBeCreatedOnlyOnce]
    )
    (
        [CreateChannel; ShutDown], 
        [NotNiceToTest; Simple ShutDownCompleted]
    )
    (
        [CreateChannel; ShutDown; ShutDown], 
        [NotNiceToTest; Simple ShutDownCompleted; Simple (ShutDownError ["Operation is not valid due to the current state of the object."]) ]
    )
    (
        [
            GetState
            CreateChannel
            ShutDown
            GetState
            CreateChannel
            GetState
        ], 
        [
            Simple CanNotRetrieveState
            NotNiceToTest 
            Simple ShutDownCompleted
            Simple (State ChannelState.Shutdown)
            Simple CanBeCreatedOnlyOnce 
            Simple (State ChannelState.Shutdown)
        ]
    )
    ] |> cast2TestData 

[<Theory; MemberData("channelCmdDataTests")>]
let ``grpc channel`` cmd expected =
    let handler = channelHandler ()
    Seq.zip cmd expected
    |> Seq.iter (fun (x, y) -> 
                    let result = handler x
                    match y with
                    | Simple z -> result =! z
                    | NotNiceToTest -> 
                        match result with
                        | CallInvoker x -> 
                            x |> isNull =! false
                        | _ -> failwith "fails")
    handler ShutDown |> ignore

[<Fact>]
let ``push rate`` () =
    writeType typeof<StatusCode>
