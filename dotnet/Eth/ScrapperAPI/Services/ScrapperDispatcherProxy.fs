namespace ScrapperAPI.Services

module ScrapperDispatcherProxy =

  open Dapr.Actors
  open Dapr.Actors.Client
  open ScrapperModels

  let private getActorId contractAddress versionId =
    let actorId = $"{contractAddress}_{versionId}"
    ActorId actorId

  let private createActor contractAddress versionId =
    let actorId = getActorId contractAddress versionId

    ActorProxy.Create<IScrapperDispatcherActor>(actorId, "scrapper-dispatcher")


  let start contractAddress abi versionId =

    let actor = createActor contractAddress versionId

    let data: StartData =
      { ContractAddress = contractAddress
        Abi = abi }

    actor.Start data


  let state contractAddress versionId =

    let actor = createActor contractAddress versionId

    actor.State()
