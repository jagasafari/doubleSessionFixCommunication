module FixServerCommunication

open QuickFix
open QuickFix.Fields
open Client.Model

let appHandle updateLogonMsg = function
    | ToAdmin msg -> updateLogonMsg msg
    | OnLogon _ | OnLogout _ | OnCreate _ | FromAdmin _ | ToApp _ | FromApp _ -> ()

let fillLogonMsg decrypt config (msg: QuickFix.Message) =
    match msg with
    | :? FIX42.Logon as l ->
        let (hb, ur) = config
        l.EncryptMethod <- EncryptMethod 0
        l.HeartBtInt <- HeartBtInt hb
        l.SetField(Username ur)
        l.SetField(Password (decrypt ()))
    | _ -> ()

