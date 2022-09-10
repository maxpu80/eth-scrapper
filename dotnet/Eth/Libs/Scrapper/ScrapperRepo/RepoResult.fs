namespace Scrapper.Repo

open System.Threading.Tasks

type RepoError =
  | NotFound
  | Frobidden

type RepoResult'<'a> = Result<'a, RepoError>
type RepoResult<'a> = Task<RepoResult'<'a>>

[<RequireQualifiedAccess>]
module RepoResult =

  let fromOption'<'a> (opt: 'a option) =
    match opt with
    | Some x -> x |> Ok
    | None -> NotFound |> Error

  let fromOption<'a> x =
    task {
      let! x = x
      return fromOption'<'a> x
    }
