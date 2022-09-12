namespace ScrapperAPI.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open Microsoft.AspNetCore.Authorization
open Common.DaprState
open Scrapper.Repo.PeojectsRepo

[<ApiController>]
[<Route("projects")>]
type ProjectsController(env: DaprStoreEnv) =
  inherit ControllerBase()
  let repo = createRepo env

  [<HttpPost>]
  member this.Post(data: CreateProjectEntity) = repo.Create data

  [<HttpGet>]
  member this.GetAll() = repo.GetAllWithVerions()

  [<HttpDelete("{projectId}")>]
  member this.Delete(projectId: string) = repo.Delete projectId
