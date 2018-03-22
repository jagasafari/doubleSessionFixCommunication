module PublishRates

open System
open System.Collections.Generic
open Grpc.Core
open Client.Model
open PublishRatesContract
open FSharp.Control.Reactive

let dispose (x: IDisposable) = x.Dispose()

let logChannel info warn error msg =
    let logMsg = sprintf "GrpcChannel|Message=%A" msg
    match msg with
    | State _
    | ShutDownCompleted -> info logMsg
    | NothingToShutDown -> warn logMsg
    | CanBeCreatedOnlyOnce
    | CanNotRetrieveState
    | ShutDownError _
    | ShutDownTimeout
    | InvokerNotCreated
    | CallInvoker _ -> ()

let logPushRate info error rate msg =
    let logMsg = sprintf "GrpcChannel|Message=%A|Rate=%A" msg rate
    match msg with
    | RatePushed -> info logMsg
    | PushingErrors _ -> error logMsg

let tryCallAsync f error =
    try f ()
    with
    | :? AggregateException as ae ->
        let errs = List<string>()
        ae.Flatten().InnerExceptions
        |> Seq.iter (fun e -> errs.Add e.Message)
        errs |> Seq.toList |> error
    | e -> error [e.Message]

let channel timeout host port =
    let mutable channel: Channel option = None
    let create () = 
        match channel with
        | None -> 
            let credentials = 
                ChannelCredentials.Insecure
            let c = Channel(host, port, credentials)
            c.ConnectAsync 
                (Nullable(DateTime.UtcNow + timeout))
            |> Async.AwaitTask
            |> ignore
            channel <- Some c
            c |> DefaultCallInvoker |> CallInvoker
        | _ -> CanBeCreatedOnlyOnce
    let shutDown () = 
        let shut () =
            match channel with
            | Some c -> 
                c.ShutdownAsync().Wait timeout
                |> function 
                | true -> ShutDownCompleted
                | false -> ShutDownTimeout
            | _ -> NothingToShutDown
        tryCallAsync shut ShutDownError
    let getState () = 
        match channel with
        | Some c -> State c.State
        | None -> CanNotRetrieveState
    function
    | CreateChannel -> create ()
    | ShutDown -> shutDown ()
    | GetState -> getState ()

let startChannel logChannel channelHandler = 
    match channelHandler CreateChannel with
    | CallInvoker invoker -> invoker
    | x -> 
        logChannel x 
        "FatalError|Message=PublishRatesChannelNotCreated"
        |> failwith

let getRatesStreamingCall (invoker: DefaultCallInvoker) =
    invoker.AsyncClientStreamingCall(
                ratesStreamingContract, null, CallOptions ())

let writeRate (call: AsyncClientStreamingCall<_,_>) rate = 
    call.RequestStream.WriteAsync rate
    |> Async.AwaitTask
    |> Async.RunSynchronously

let getRatesStreaming push rate =
    tryCallAsync (fun () -> push rate; RatePushed) PushingErrors

let pushRate logPush invoker rate = 
    let push = invoker |> getRatesStreamingCall |> writeRate
    getRatesStreaming push rate |> logPush rate

let simulatePush push mockRate = 
    let mutable subs: IDisposable option = None
    let start invoker =
        let push' _ = push invoker mockRate
        subs <-
            TimeSpan.FromMilliseconds 1.
            |> Observable.timerPeriod DateTimeOffset.Now
            |> Observable.subscribe push'
            |> Some
    let stop () = subs |> Option.iter dispose 
    start, stop 
