namespace Common.DaprAPI

[<AutoOpen>]
module RepoResultFilter =

  open Common.Repo
  open Microsoft.AspNetCore.Mvc
  open Microsoft.FSharp.Reflection
  open Microsoft.AspNetCore.Mvc.Filters

  let private mapRepoError =
    function
    | NotFound -> () |> NotFoundObjectResult :> ActionResult
    | Frobidden -> () |> ForbidResult :> ActionResult
    | Conflict -> () |> ConflictObjectResult :> ActionResult

  let private mapActionResult (actionResult: IActionResult) =
    let objectResult = actionResult |> box :?> ObjectResult
    let objectResultValue = objectResult.Value

    match FSharpValue.GetUnionFields(objectResultValue, objectResultValue.GetType()) with
    | x, vals when x.Name = "Error" ->
      match vals[0] with
      | :? RepoError as err -> err |> mapRepoError
      | _ -> 500 |> StatusCodeResult :> ActionResult
    | _, vals -> vals[0] |> OkObjectResult :> ActionResult

  type RepoResultFilter() =
    interface IAsyncResultFilter with
      member this.OnResultExecutionAsync(ctx, next) =
        task {

          ctx.Result <- mapActionResult ctx.Result

          return! next.Invoke()
        }
