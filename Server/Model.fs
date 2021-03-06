namespace Server.Model

type AppConfig =
    {
    QuickFixConfigFile: string
    }

type PriceSide = | Bid | Ask

type Connection = interface end

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
