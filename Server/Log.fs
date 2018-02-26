module Log

open Server.Model
open Common.Common

let logQuickFixMsg info = function
    | OnEvent x -> x |> sprintf "Event|event=%s" |> info
    | OnOutgoing x -> x |> parseFixMsg |> sprintf "out|%s" |> info
    | OnIncoming x -> x |> parseFixMsg |> sprintf "in|%s" |> info

let logAppMsg info msg =
    let name = getUnionCaseName<ApplicationMsg> msg
    let logMsg = parseFixMsg >> sprintf "%s|Message=%s" name >> info
    let logSessionId = sprintf "%s|SessionId=%A" name >> info
    match msg with
    | OnCreate x | OnLogon x | OnLogout x -> logSessionId x
    | ToAdmin x | FromAdmin x | ToApp x | FromApp x -> logMsg x

let logError log = sprintf "%A" >> log
