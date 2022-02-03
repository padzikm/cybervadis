namespace Foosball.Api

open System
open System.Collections.Generic
open System.IO
open System.Linq
open System.Threading.Tasks
open Foosball.Api.Controllers
open Foosball.Application.ScoreGoal.Types
open Microsoft.AspNetCore
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open NServiceBus

module Program =
    let exitCode = 0

    let CreateHostBuilder args =
        Host.CreateDefaultBuilder(args)
            .UseNServiceBus(fun ctx ->
                let endpoint = EndpointConfiguration("Api")
                endpoint.SendOnly()
                let t = endpoint.UseTransport<LearningTransport>()
                let r = t.Routing()
                r.RouteToEndpoint(typeof<ScoreGoalCommand>.Assembly, "Games") |> ignore
                endpoint
                )
            .ConfigureWebHostDefaults(fun webBuilder ->
                webBuilder.UseStartup<Startup>() |> ignore
            )

    [<EntryPoint>]
    let main args =
        CreateHostBuilder(args).Build().Run()

        exitCode
