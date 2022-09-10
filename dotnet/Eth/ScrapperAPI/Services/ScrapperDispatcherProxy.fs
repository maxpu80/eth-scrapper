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
        let actor = createActor proj.Address ver.Id

        let data: StartData =
          { ContractAddress = proj.Address
            Abi = proj.Abi }

        let! result = actor.Start data

        match result with
        | true ->
          let! result = state proj.Address ver.Id

          match result.Data with
          | Some state -> return Ok state
          | None -> return AfterActorStartStateNotFound |> Error
        | false -> return ActorStartFailure |> Error
      | Error err -> return err |> RepoError |> Error

    }
