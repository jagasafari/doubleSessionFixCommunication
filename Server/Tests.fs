module Tests

open System
open Xunit
open log4net
open System.IO
open Common.SignatureUtil
open System.Reflection
open QuickFix
    
[<Fact>]
let ``sig`` () =
    writeTypeMembers typeof<SessionID>
