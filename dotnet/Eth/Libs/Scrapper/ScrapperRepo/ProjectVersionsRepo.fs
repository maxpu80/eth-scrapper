namespace Scrapper.Repo

open System

module ProjectVersionsRepo =
  open Common.DaprState.StateList
  open Common.Repo.RepoResult

  type VersionEntity = { Id: string; Created: DateTime }

  let getKey projId = $"project_{projId}_versions"

  let createRepo env =
    let repo = stateListRepo<VersionEntity> env

    {| Create =
        fun projId enty ->
          repo.Insert (getKey projId) (fun x -> x.Id = enty.Id) enty
          |> taskMap errorToConflict
       GetAll = fun projId -> repo.GetAll(getKey projId)
       Delete =
        fun projId id ->
          repo.Delete (getKey projId) (fun enty -> enty.Id = id)
          |> taskMap noneToNotFound
       DeleteAll = fun projId -> repo.DeleteAll(getKey projId) |}
