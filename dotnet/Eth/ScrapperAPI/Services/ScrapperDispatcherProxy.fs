namespace ScrapperAPI.Services

module ScrapperDispatcherProxy =

  open Dapr.Actors
  open Dapr.Actors.Client
  open ScrapperModels
  open Scrapper.Repo.PeojectsRepo
  open Common.DaprState
  open Common.Repo

  let private getActorId contractAddress versionId =
    let actorId = $"{contractAddress}_{versionId}"
    ActorId actorId

  let private createActor contractAddress versionId =
    let actorId = getActorId contractAddress versionId

    ActorProxy.Create<IScrapperDispatcherActor>(actorId, "scrapper-dispatcher")

  let state contractAddress versionId =

    let actor = createActor contractAddress versionId

    actor.State()

  let pause contractAddress versionId =

    let actor = createActor contractAddress versionId

    actor.Pause()

  let resume contractAddress versionId =

    let actor = createActor contractAddress versionId

    actor.Resume()

  let reset contractAddress versionId =

    let actor = createActor contractAddress versionId

    actor.Reset()

  type StartError =
    | ActorStartFailure
    | AfterActorStartStateNotFound
    | RepoError of RepoError

  let start (env: DaprStoreEnv) projectId versionId =
    let repo = createRepo env

    task {

      let! result = repo.GetOneWithVersion projectId versionId

      match result with
      | Ok (proj, ver) ->
        let actor = createActor projectId ver.Id

        let data: StartData =
          { EthProviderUrl = proj.EthProviderUrl
            ContractAddress = proj.Address
            Abi = proj.Abi }

        let! result = actor.Start data

        match result with
        | true ->
          let! result = state projectId ver.Id

          match result.Data with
          | Some state -> return Ok state
          | None -> return AfterActorStartStateNotFound |> Error
        | false -> return ActorStartFailure |> Error
      | Error err -> return err |> RepoError |> Error

    }
