namespace Client.Model

type Connection = interface end

type LogMsg = 
    | FixEvent of string 
    | FixMsgOutgoing of string
    | FixMsgIncoming of string

