namespace Common.Repo

open System.Threading.Tasks

type RepoError =
  | NotFound
  | Frobidden
  | Conflict

type RepoResult'<'a> = Result<'a, RepoError>
type RepoResult<'a> = Task<RepoResult'<'a>>

module RepoResult =

  let noneTo'<'a> t (opt: 'a option) =
    match opt with
    | Some x -> x |> Ok
    | None -> t |> Error

  let noneToNotFound<'a> = noneTo'<'a> NotFound

  let noneToConflict<'a> = noneTo'<'a> Conflict

  let taskMap fn t =
    task {
      let! x = t
      return fn x
    }
