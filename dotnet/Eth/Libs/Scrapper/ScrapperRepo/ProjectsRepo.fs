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
  let USER_KEY = "projects_USER_KEY"

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
          repo.Delete USER_KEY (fun enty -> enty.Id = id)
          |> taskMap noneToNotFound |}
