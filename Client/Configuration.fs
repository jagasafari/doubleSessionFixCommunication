module Configuration 

open System
open Microsoft.Extensions.Configuration
open DataTypes

let appConfig =
    lazy
        let builder: IConfigurationRoot =
            ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional = true)
                .Build()

        { ReactIntervalStart = builder.["ReactIntervalStart"] |> Int32.Parse
          ReactIntervalRun = builder.["ReactIntervalRun"] |> Int32.Parse
          ReactIntervalStop = builder.["ReactIntervalStop"] |> Int32.Parse }
