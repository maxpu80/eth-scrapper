namespace ScrapperAPI

#nowarn "20"

open System
open System.Collections.Generic
open System.IO
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.HttpsPolicy
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open System.Security.Claims
open System.Text.Json
open System.Text.Json.Serialization
open Microsoft.Extensions.Configuration
open Dapr.Client
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Mvc.Filters
open Microsoft.FSharp.Reflection
open Common.DaprState
open Common.DaprAPI

module Program =
  let exitCode = 0

  let getDaprAppEnv (serviceProvider: IServiceProvider) =
    let loggerFactory = serviceProvider.GetService<ILoggerFactory>()
    let daprClient = serviceProvider.GetService<DaprClient>()
    let logger = loggerFactory.CreateLogger()
    { Logger = logger; Dapr = daprClient }


  [<EntryPoint>]
  let main args =

    let builder = WebApplication.CreateBuilder(args)

    Common.DaprAPI.DaprLogging.createSerilogLogger builder.Configuration builder.Host

    let services = builder.Services

    let converter =
      JsonFSharpConverter(
        JsonUnionEncoding.ExternalTag
        ||| JsonUnionEncoding.UnwrapSingleCaseUnions,
        allowNullFields = true
      )

    services
      .AddControllers(fun opts -> opts.Filters.Add(RepoResultFilter()))
      .AddJsonOptions(fun opts ->
        opts.JsonSerializerOptions.PropertyNameCaseInsensitive <- true
        opts.JsonSerializerOptions.PropertyNamingPolicy <- JsonNamingPolicy.CamelCase
        opts.JsonSerializerOptions.Converters.Add(converter))


    services.AddDaprClient (fun builder ->
      let jsonOpts = JsonSerializerOptions()
      jsonOpts.PropertyNameCaseInsensitive <- true
      jsonOpts.PropertyNamingPolicy <- JsonNamingPolicy.CamelCase
      jsonOpts.Converters.Add(converter)

      builder.UseJsonSerializationOptions(jsonOpts)
      |> ignore)

    services.AddScoped<DaprAppEnv>(Func<_, _>(getDaprAppEnv))

    services.AddScoped<DaprStoreEnv>(
      Func<_, _> (fun (serviceProvider: IServiceProvider) ->
        let app = getDaprAppEnv serviceProvider

        let storeName =
          builder
            .Configuration
            .GetSection("Dapr")
            .GetValue<string>("StoreName")

        { App = app; StoreName = storeName })
    )

    let app = builder.Build()

    app.UseCors (fun x ->
      x
        .AllowAnyMethod()
        .AllowAnyHeader()
        .SetIsOriginAllowed(fun _ -> true)
        .AllowCredentials()
      |> ignore)


    app.MapControllers()

    let port = System.Environment.GetEnvironmentVariable("PORT")

    let url = $"http://*:{port}"

    app.Run(url)

    exitCode
