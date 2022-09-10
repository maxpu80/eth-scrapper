namespace ScrapperAPI.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open Microsoft.AspNetCore.Authorization
open Common.DaprState
open Scrapper.Repo.ProjectVersionsRepo

[<ApiController>]
[<Route("projects/{projectId}/versions")>]
type ProjectVersionssController(env: DaprStoreEnv) =
  inherit ControllerBase()
  let repo = createRepo env

  [<HttpPost>]
  member this.Post(projectId: string, data: VersionEntity) = repo.Create projectId data

  [<HttpGet>]
  member this.GetAll(projectId: string) = repo.GetAll projectId

  [<HttpDelete("{versionId}")>]
  member this.Delete(projectId: string, versionId: string) = repo.Delete projectId versionId

