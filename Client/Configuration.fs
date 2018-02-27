module Configuration 

open System
open Microsoft.Extensions.Configuration
open Client.Model

let appConfig =
    lazy
        let builder: IConfigurationRoot =
            ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional = true)
                .Build()

        { 
        QuickFixConfigFile = builder.["QuickFixConfigFile"] 
        HeartbeatFrequency = Int32.Parse builder.["HeartbeatFrequency"]
        User = builder.["User"]
        Password = builder.["Password"]
        }
