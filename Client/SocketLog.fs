module SocketLog

open System
open Client.Model
open Util

let parseFixMsg (msg: obj) = 
    let replace (x: string) = 
        x.Replace('\u0001', '|')
    match msg with
    | null -> String.Empty
    | :? string as x -> replace x
    | :? QuickFix.Message as x -> 
        x.ToString() |> replace
    | x -> x |> sprintf "%A"

let isPricingMsg = function 
    | null -> false
    | (msg: string) when msg.Contains("35=W") -> true
    | _ -> false

let logQuickFixMsg debug info = function
    | OnEvent x -> 
        x |> sprintf "Event|event=%s" |> info
    | OnOutgoing x -> 
        x |> parseFixMsg |> sprintf "out|%s" |> info
    | OnIncoming x -> 
            parseFixMsg x 
            |> sprintf "in|%s" 
            |> if isPricingMsg x then debug else info

let logAppMsg info msg =
    let name = getUnionCaseName<ApplicationMsg> msg
    let logMsg = 
        parseFixMsg 
        >> sprintf "%s|Message=%s" name 
        >> info
    let logSessionId = 
        sprintf "%s|SessionId=%A" name >> info
    match msg with
    | OnCreate x | OnLogon x | OnLogout x -> logSessionId x
    | ToAdmin x | FromAdmin x | ToApp x | FromApp x -> 
        logMsg x
