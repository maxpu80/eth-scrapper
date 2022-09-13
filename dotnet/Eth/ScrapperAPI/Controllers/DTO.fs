﻿namespace ScrapperAPI.Controllers

module private DTO =
  open ScrapperModels
  open Scrapper.Repo.PeojectsRepo
  open Microsoft.AspNetCore.Mvc


  let toNullable =
    function
    | Some x -> x |> box
    | None -> null

  let mapState (state: State) =
    {| request =
        {| state.Request with
             BlockRange =
               {| From = state.Request.BlockRange.From |> Option.toNullable
                  To = state.Request.BlockRange.To |> Option.toNullable |} |}
       date = state.Date
       finishDate = state.FinishDate
       status =
        match state.Status with
        | Status.Continue -> "continue"
        | Status.Pause -> "pause"
        | Status.Finish -> "finish"
        | Status.Schedule -> "schedule" |}

  let mapProjectsWithViewStates (projects: ProjectWithVresionsAndState list) =
    projects
    |> List.map (fun project ->
      {| project with
           Versions =
             project.Versions
             |> List.map (fun version ->
               {| version with
                    State = version.State |> Option.map mapState |> toNullable |}) |})

  let mapScrapperDispatcherActorResult (result: ScrapperDispatcherActorResult) =
    match result with
    | Ok state -> state |> mapState |> OkObjectResult :> IActionResult
    | Error err ->
      match err with
      | StateConflict (state, error) ->
        ConflictObjectResult(
          {| State = (mapState state)
             Error = error |}
        )
        :> IActionResult
      | StateNotFound -> NotFoundObjectResult() :> IActionResult
