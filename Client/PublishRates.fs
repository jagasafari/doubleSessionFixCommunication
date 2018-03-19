module PublishRates

open System
open System.Collections.Generic
open Grpc.Core
open Client.Model
open PublishRatesContract

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


let deadline = (+) DateTime.UtcNow

let tryCallAsync f error =
    try f ()
    with
    | :? AggregateException as ae ->
        let errs = List<string>()
        ae.Flatten().InnerExceptions
        |> Seq.iter (fun e -> errs.Add e.Message)
        errs |> Seq.toList |> error
    | e -> error [e.Message]

type PushRateResult = | RatePushed | PushingErrors of string list

let getRatesStreaming (invoker: DefaultCallInvoker) timeout =
    let callOptions = 
        timeout |> deadline |> CallOptions().WithDeadline
    let call =
        invoker.AsyncClientStreamingCall(
                    ratesStreamingContract, null, callOptions)
    let pushRate rate () =
        call.RequestStream.WriteAsync rate 
        |> Async.AwaitTask
        |> Async.RunSynchronously
        RatePushed
    let push rate = 
        tryCallAsync (pushRate rate) PushingErrors
    push

let channel timeout host port =
    let mutable channel: Channel option = None
    let create () = 
        match channel with
        | None -> 
            let credentials = 
                ChannelCredentials.Insecure
            let c = Channel(host, port, credentials)
            c.ConnectAsync (Nullable(deadline timeout))
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
