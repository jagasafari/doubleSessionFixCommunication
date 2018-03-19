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
        HeartbeatFrequency = builder.["HeartbeatFrequency"] |> int
        User = builder.["User"]
        Password = builder.["Password"]
        RatePushingDeadline = 
            builder.["RatePushingDeadline"] 
            |> float 
            |> TimeSpan.FromMilliseconds
        PublishRatesHost = builder.["PublishRatesHost"]
        PublishRatesPort = builder.["PublishRatesPort"] |> int
        }
