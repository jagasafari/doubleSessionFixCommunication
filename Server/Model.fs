namespace Server.Model

type AppConfig =
    {
    QuickFixConfigFile: string
    }

type PriceSide = | Bid | Ask

type FixServerConnection = interface end

type LogMsg = 
    | LogEvent of string 
    | LogOutgoing of string
    | LogIncoming of string

