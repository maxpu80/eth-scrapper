﻿namespace ScrapperAPI.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open Microsoft.AspNetCore.Authorization
open Common.DaprState
open Scrapper.Repo
open Scrapper.Repo.PeojectsRepo
open ScrapperAPI.Services
open Common.DaprActor.ActorResult


[<ApiController>]
[<Route("projects/{projectId}/versions")>]
type ProjectVersionssController(env: DaprStoreEnv) =
  inherit ControllerBase()
  let repo = createRepo env

  [<HttpPost>]
  member this.Post(projectId: string, data: CreateVersionEntity) = repo.CreateVersion projectId data

  [<HttpGet>]
  member this.GetAll(projectId: string) = repo.GetAllVersions projectId

  [<HttpDelete("{versionId}")>]
  member this.Delete(projectId: string, versionId: string) = repo.DeleteVersion projectId versionId

  [<HttpPost("{versionId}/start")>]
  member this.Start(projectId: string, versionId: string) =
    task {
      let! result = repo.GetOneWithVersion projectId versionId

      match result with
      | Ok (proj, ver) ->
        let! result = ScrapperDispatcherProxy.start proj.Address proj.Abi ver.Id

        match result with
        | true -> return! ScrapperDispatcherProxy.state proj.Address ver.Id
        | false -> return actorOptionResultNone
      | _ as err -> return actorOptionResultNone
    }
