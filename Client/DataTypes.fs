module DataTypes

open QuickFix

type AppConfig =
    { ReactIntervalStart: int
      ReactIntervalRun: int
      ReactIntervalStop: int }

type App () =
    interface IApplication with
        member this.OnCreate(sessionId) = ()
        member this.FromAdmin(message, sessionId) = ()    
        member this.ToAdmin(message, sessionId) = ()    
        member this.OnLogon(sessionId) = ()    
        member this.OnLogout(sessionId) = ()    
        member this.FromApp(message, sessionId) = ()    
        member this.ToApp(message, sessionId) = ()    
