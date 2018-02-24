module Configuration 

open System
open Microsoft.Extensions.Configuration

type AppConfig =
    {
    QuickFixConfigFile: string
    }

let appConfig =
    lazy
        let builder: IConfigurationRoot =
            ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional = true)
                .Build()

        { QuickFixConfigFile = builder.["QuickFixConfigFile"] }
