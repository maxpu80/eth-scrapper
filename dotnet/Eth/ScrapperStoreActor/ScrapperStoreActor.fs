namespace ScrapperStoreActor

[<AutoOpen>]
module ScrapperStoreActor =

  open Dapr.Actors
  open Dapr.Actors.Runtime
  open System.Threading.Tasks
  open ScrapperModels

  let store () (data: ContinueSuccessData) =
    task {
      printfn "Store events: %i" data.Result.Events.Length
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
  type ScrapperStoreActor(host: ActorHost) =
    inherit Actor(host)

    interface IScrapperStoreActor with
      member this.Store data =
        task {
          do! store () data
          runScrapperDispatcher this.ProxyFactory this.Id data
          return true
        }

      member this.Test data =
        task {
          printfn "+++ %O" data
          return true
        }
