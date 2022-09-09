namespace Common.DaprActor

open Microsoft.Extensions.Hosting

[<AutoOpen>]
module DaprLogging =

  open Microsoft.Extensions.Configuration
  open Microsoft.AspNetCore.Hosting
  open System
  open Serilog
  open Serilog.Sinks.Elasticsearch
  open Serilog.Enrichers.Span

  let createSerilogLogger (configuration: IConfiguration) (webHostBuilder: IHostBuilder) =

    let mutable logger =
      LoggerConfiguration()
        .ReadFrom.Configuration(configuration)
        .Enrich.FromLogContext()
        .Enrich.WithSpan()

    //let elasticHost = configuration.GetConnectionString("ElasticHostLogger")
    //let seqHost = configuration.GetConnectionString("SeqHostLogger")

    //if String.IsNullOrEmpty(elasticHost) |> not then
    //  logger <- logger.WriteTo.Elasticsearch(ElasticsearchSinkOptions(Uri elasticHost))

    //printfn "!!! %s" seqHost
    //if String.IsNullOrEmpty(seqHost) |> not then
    //  logger <- logger.WriteTo.Seq(seqHost)

    Log.Logger <- logger.CreateLogger()
    webHostBuilder.UseSerilog()
