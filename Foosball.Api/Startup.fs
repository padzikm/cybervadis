namespace Foosball.Api

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Foosball.Application.CreateGame.Types
open Foosball.Application.Database
open Foosball.Application.Database
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.HttpsPolicy;
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.EntityFrameworkCore
open MediatR
open Hellang.Middleware.ProblemDetails

type Startup(configuration: IConfiguration) =
    member _.Configuration = configuration

    // This method gets called by the runtime. Use this method to add services to the container.
    member _.ConfigureServices(services: IServiceCollection) =
        // Add framework services.
        services.AddControllers() |> ignore
        services.AddMediatR(typeof<CreateGameCommand>) |> ignore
        services.AddProblemDetails() |> ignore
        services.AddScoped(typeof<IRepository>, typeof<Repository>) |> ignore

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    member _.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =
        app.UseProblemDetails() |> ignore
//        if (env.IsDevelopment()) then
//            app.UseDeveloperExceptionPage() |> ignore
//        app.UseHttpsRedirection()
        app.UseRouting()
           .UseAuthorization()
           .UseEndpoints(fun endpoints ->
                endpoints.MapControllers() |> ignore
            ) |> ignore
