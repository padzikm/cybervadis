namespace Foosball.Service.Program

open System
open System.Collections.Generic
open System.Data.Common
open System.Linq
open System.Threading.Tasks
open Foosball.Application.Database
open Microsoft.Extensions.Hosting
open NServiceBus
open NServiceBus.ObjectBuilder
open NServiceBus
open Microsoft.Extensions.DependencyInjection

module Program =
    let createHostBuilder args =
        Host.CreateDefaultBuilder(args)
            .UseNServiceBus(fun ctx ->
                let endpoint = EndpointConfiguration("Games")
                let _ = endpoint.UseTransport<LearningTransport>()
                endpoint
                )
            .ConfigureServices(fun hostContext services ->
                services.AddScoped(typeof<IRepository>, typeof<Repository>) |> ignore
                )

    [<EntryPoint>]
    let main args =
        createHostBuilder(args).Build().Run()

        0 // exit code