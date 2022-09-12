namespace ScrapperElasticStoreActor

[<AutoOpen>]
module ScrapperStoreActor =

  open Dapr.Actors
  open Dapr.Actors.Runtime
  open ScrapperModels
  open Microsoft.Extensions.Configuration
  open Elasticsearch.Net
  open Microsoft.Extensions.Logging
  open Common.DaprActor

  let elasticConfig (config: IConfiguration) =
    let connectionString = config.GetConnectionString("ElasticSearch")
    connectionString

  let store (logger: ILogger) (elasticConfig: string) (indexPayload: string) =

    logger.LogDebug("Store scrapper payload to elasticsearch {config}", elasticConfig)

    let elasticConfig = new ConnectionConfiguration(System.Uri(elasticConfig))
    let client = ElasticLowLevelClient(elasticConfig)

    let data = PostData.String(indexPayload)

    task {
      try
        let! response = client.BulkAsync<VoidResponse> data

        logger.LogInformation(
          "Store scrapper payload to elasticsearch success {success} {@error}",
          response.Success,
          response.OriginalException
        )

        if not response.Success then
          raise response.OriginalException

        return ()
      with
      | _ as err ->
        logger.LogError("Store scrapper payload to elasticsearch error {@error}", err)
        return ()

    }

  let runScrapperDispatcher (proxyFactory: Client.IActorProxyFactory) id (data: ContinueSuccessData) =
    let actor =
      proxyFactory.CreateActorProxy<IScrapperDispatcherActor>(id, "scrapper-dispatcher")

    let success = data.Result

    let continueData: ContinueData =
      { EthProviderUrl = data.EthProviderUrl
        ContractAddress = data.ContractAddress
        Abi = data.Abi
        Result =
          (Ok
            { BlockRange = success.BlockRange
              RequestBlockRange = success.RequestBlockRange }) }

    actor.Continue continueData |> ignore


  [<Actor(TypeName = "scrapper-elastic-store")>]
  type ScrapperElasticStoreActor(host: ActorHost, config: IConfiguration) =
    inherit Actor(host)
    let logger = ActorLogging.create host

    interface IScrapperStoreActor with
      member this.Store data =
        task {
          let config = elasticConfig config
          do! store logger config data.Result.IndexPayload
          runScrapperDispatcher this.ProxyFactory this.Id data
          return true
        }
