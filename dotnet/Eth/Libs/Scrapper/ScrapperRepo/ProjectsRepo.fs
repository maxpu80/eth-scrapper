namespace Scrapper.Repo

module PeojectsRepo =
  open Common.DaprState.StateList
  open Common.Repo

  type ProjectEntity =
    { Id: string
      Name: string
      Address: string
      Abi: string }


  // TODO : Multi user env
  let USER_KEY = "user_USER_KEY_projects"

  let createRepo env =
    let repo = stateListRepo<ProjectEntity> env
    let versionRepo = ProjectVersionsRepo.createRepo env

    let getOne projId =
      repo.GetHead USER_KEY (fun x -> x.Id = projId)

    {| Create =
        fun enty ->
          repo.Insert USER_KEY (fun x -> x.Id = enty.Id) enty
          |> taskMap errorToConflict
       GetAll = fun () -> repo.GetAll USER_KEY |> taskMap Ok
       GetOneWithVersion =
        fun projId verId ->
          task {
            let! proj = getOne projId

            match proj with
            | Some proj ->
              let! ver = versionRepo.GetOne projId verId

              match ver with
              | Some ver -> return Some(proj, ver)
              | None -> return None
            | None -> return None
          }
          |> taskMap noneToNotFound
       Update =
        fun id enty ->
          repo.Update USER_KEY (fun enty -> enty.Id = id) (fun _ -> enty)
          |> taskMap noneToNotFound
       Delete =
        fun id ->
          task {
            let! result = repo.Delete USER_KEY (fun enty -> enty.Id = id)

            match result with
            | Some _ -> do! versionRepo.DeleteAll id
            | None -> ()

            return result |> noneToNotFound
          }
       CreateVersion =
        fun id ver ->
          task {
            let! proj = getOne id

            match proj with
            | Some _ -> return! versionRepo.Create id ver
            | None -> return RepoError.NotFound |> Error
          }
       DeleteVersion = fun id verId -> versionRepo.Delete id verId
       GetAllVersions = fun id -> versionRepo.GetAll id |}
