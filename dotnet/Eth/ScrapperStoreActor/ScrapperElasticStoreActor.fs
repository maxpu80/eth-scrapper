namespace ScrapperElasticStoreActor

[<AutoOpen>]
module ScrapperStoreActor =

  open Dapr.Actors
  open Dapr.Actors.Runtime
  open System.Threading.Tasks
  open ScrapperModels
  open Infra.AzureBlob
  open Microsoft.Extensions.Configuration
  open Elasticsearch.Net
  open Microsoft.Extensions.Logging

  let elasticConfig (config: IConfiguration) =
    let connectionString = config.GetConnectionString("ElasticSearch")
    connectionString

  let store (logger: ILogger) elasticConfig (indexPayload: string) =

    logger.LogDebug("Store scrapper payload to elasticsearch")

    use elasticConfig = new ConnectionConfiguration(System.Uri(elasticConfig))
    let client = ElasticLowLevelClient(elasticConfig)

    let data = PostData.String(indexPayload)

    task {
      try
        let! response = client.BulkAsync<VoidResponse> data

        logger.LogInformation(
          "Store scrapper payload to elasticsearch success {success} @{error}",
          response.Success,
          response.OriginalException
        )

        return ()
      with
      | _ as err ->
        logger.LogError("Store scrapper payload to elasticsearch error @{error}", err)
        return raise err

    }

  let runScrapperDispatcher (proxyFactory: Client.IActorProxyFactory) id (data: ContinueSuccessData) =
    let actor =
      proxyFactory.CreateActorProxy<IScrapperDispatcherActor>(id, "scrapper-dispatcher")

    let success = data.Result

    let continueData: ContinueData =
      { ContractAddress = data.ContractAddress
        Abi = data.Abi
        Result =
          (Ok
            { BlockRange = success.BlockRange
              RequestBlockRange = success.RequestBlockRange }) }

    actor.Continue continueData |> ignore


  [<Actor(TypeName = "scrapper-store")>]
  type ScrapperElasticStoreActor(host: ActorHost, config: IConfiguration) =
    inherit Actor(host)

    interface IScrapperStoreActor with
      member this.Store data =
        let logger = this.Host.LoggerFactory.CreateLogger()

        task {
          let config = elasticConfig config
          do! store logger config data.Result.IndexPayload
          runScrapperDispatcher this.ProxyFactory this.Id data
          return true
        }
