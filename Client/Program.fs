module Program 

open System
open Client

let [<EntryPoint>] main _ = 
    let configPath = "fix.cfg"
    let start, stop = createSocket configPath
    start ()
    Console.ReadKey () |> ignore
    stop ()
    0
