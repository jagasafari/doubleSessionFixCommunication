namespace Client.Model

open System
open Grpc.Core

type PushRateResult = | RatePushed | PushingErrors of string list

type ChannelMessage = 
    | CreateChannel 
    | ShutDown 
    | GetState

type ChannelResult = 
    | CanNotRetrieveState
    | State of ChannelState
    | ShutDownCompleted
    | CanBeCreatedOnlyOnce
    | ShutDownError of string List
    | ShutDownTimeout
    | NothingToShutDown
    | InvokerNotCreated
    | CallInvoker of DefaultCallInvoker

type Connection = interface end

type SubscriptionCacheChange<'a> =
    | Refresh | Clean | Remove of 'a | UnsubscribeAll

type AppConfig =
    {
    QuickFixConfigFile: string
    HeartbeatFrequency: int
    User: string
    Password: string
    RatePushingDeadline: TimeSpan
    PublishRatesHost: string
    PublishRatesPort: int
    }

type QuicFixLoggingMsg = 
    | OnEvent of string 
    | OnOutgoing of string
    | OnIncoming of string

[<NoComparisonAttribute>]
type ApplicationMsg =
    | OnCreate of QuickFix.SessionID
    | ToAdmin of QuickFix.Message
    | FromAdmin of QuickFix.Message
    | ToApp of QuickFix.Message
    | FromApp of QuickFix.Message
    | OnLogon of QuickFix.SessionID
    | OnLogout of QuickFix.SessionID
