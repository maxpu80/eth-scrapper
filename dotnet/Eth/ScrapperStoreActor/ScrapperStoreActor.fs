namespace ScrapperStoreActor

open Microsoft.Extensions.Configuration

[<AutoOpen>]
module ScrapperStoreActor =

  open Dapr.Actors
  open Dapr.Actors.Runtime
  open System.Threading.Tasks
  open ScrapperModels
  open Infra.AzureBlob

  let getAzureBlobConfig (config: IConfiguration) =
    let connectionString = config.GetConnectionString("AzureBlob")

    //let tableName =
    //  config
    //    .GetRequiredSection("Values")
    //    .GetValue<string>("AzureBlobContainerName")

    //connectionString, tableName
    connectionString

  let store azureTableConfig (version: string) (data: ContinueSuccessData) =
    printfn "Store events: %i" data.Result.Events.Length

    task {
      try

        let containerName = $"{data.ContractAddress}-{version}".ToLower()

        printfn "container name %s" containerName

        let! blobContainerClient = blobClientOpenCreateIfNotExist (azureTableConfig, containerName)

        let id =
          data
            .Result
            .BlockRange
            .To
            .ToString()
            .PadLeft(15, '0')

        let blobName = $"{id}.json".ToLower()

        printfn "blob name %s" blobName
        
        let! _ = BlobOperators.writeString blobContainerClient blobName data.Result.Events

        printfn "Store events success"
      with
      | _ as err -> printfn "Store events error %O" err

      return ()
    }

  let runScrapperDispatcher (proxyFactory: Client.IActorProxyFactory) id (data: ContinueSuccessData) =
    let actor =
      proxyFactory.CreateActorProxy<IScrapperDispatcherActor>(id, "scrapper-dispatcher")

    let success = data.Result

    let continueData: ContinueData =
      { ContractAddress = data.ContractAddress
        Abi = data.Abi
        Result = (Ok success) }

    actor.Continue continueData |> ignore


  [<Actor(TypeName = "scrapper-store")>]
  type ScrapperStoreActor(host: ActorHost, config: IConfiguration) =
    inherit Actor(host)

    interface IScrapperStoreActor with
      member this.Store data =
        task {
          let id = this.Id.ToString()
          let azureTableConfig = getAzureBlobConfig config
          do! store azureTableConfig id data
          runScrapperDispatcher this.ProxyFactory this.Id data
          return true
        }

      member this.Test () =
        task {
          let id = this.Id.ToString()
          let azureBlobConfig = getAzureBlobConfig config

          let _data: ContinueSuccessData =
            { ContractAddress = "test"
              Abi = "abi"
              Result =
                { RequestBlockRange = { From = None; To = None }
                  BlockRange = { From = 0u; To = 0u }
                  Events = "events" } }

          do! store azureBlobConfig id _data
          return true
        }
