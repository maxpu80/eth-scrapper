namespace Scrapper.Repo

module PeojectsRepo =
  open Common.DaprState.StateList

  type ProjectEntity =
    { Id: string
      Name: string
      Address: string
      Abi: string }


  // TODO : Multi user env
  let USER_KEY = "projects_USER_KEY"

  let createRepo env =
    let repo = stateListRepo<ProjectEntity> env

    {| Create = fun enty -> repo.Set USER_KEY enty
       GetAll = fun () -> repo.GetAll USER_KEY |}
