namespace ScrapperAPI.Controllers

open Microsoft.AspNetCore.Mvc
open Common.DaprState
open Scrapper.Repo
open Scrapper.Repo.PeojectsRepo
open ScrapperAPI.Services
open ScrapperDispatcherProxy
open Common.DaprAPI

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
      let! result = ScrapperDispatcherProxy.start env projectId versionId

      match result with
      | Error err ->
        match err with
        | ActorStartFailure -> return this.StatusCode(500, {| Message = "Actor start failure" |}) :> IActionResult
        | AfterActorStartStateNotFound ->
          return this.StatusCode(500, {| Message = "Actor started but state not found" |})
        | RepoError err -> return mapRepoError err
      | Ok result ->
        return
          this.AcceptedAtAction(
            "State",
            {| ProjectId = projectId
               VersionId = versionId |},
            result
          )
          :> IActionResult
    }

  [<HttpGet("{versionId}/state")>]
  member this.State(projectId: string, versionId: string) =
    task {
      let! result = ScrapperDispatcherProxy.state projectId versionId

      match result.Data with
      | Some result -> return result |> this.Ok :> IActionResult
      | None -> return NotFoundObjectResult() :> IActionResult
    }

  [<HttpPost("{versionId}/pause")>]
  member this.Pause(projectId: string, versionId: string) =
    ScrapperDispatcherProxy.pause projectId versionId

  [<HttpPost("{versionId}/resume")>]
  member this.Resume(projectId: string, versionId: string) =
    ScrapperDispatcherProxy.resume projectId versionId

  [<HttpPost("{versionId}/reset")>]
  member this.Reset(projectId: string, versionId: string) =
    ScrapperDispatcherProxy.reset projectId versionId
