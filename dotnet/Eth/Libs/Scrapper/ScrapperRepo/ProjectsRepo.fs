namespace Scrapper.Repo

module PeojectsRepo =
  open Common.DaprState.StateList
  open Common.Repo.RepoResult

  type ProjectEntity =
    { Id: string
      Name: string
      Address: string
      Abi: string }


  // TODO : Multi user env
  let USER_KEY = "user_USER_KEY_projects"

  let createRepo env =
    let repo = stateListRepo<ProjectEntity> env

    {| Create =
        fun enty ->
          repo.Insert USER_KEY (fun x -> x.Id = enty.Id) enty
          |> taskMap errorToConflict
       GetAll = fun () -> repo.GetAll USER_KEY
       Update =
        fun id enty ->
          repo.Update USER_KEY (fun enty -> enty.Id = id) (fun _ -> enty)
          |> taskMap noneToNotFound
       Delete =
        fun id ->
          task {
            let! result = repo.Delete USER_KEY (fun enty -> enty.Id = id)

            match result with
            | Some _ -> do! ((ProjectVersionsRepo.createRepo env).DeleteAll id)
            | None -> ()

            return result |> noneToNotFound
          } |}
