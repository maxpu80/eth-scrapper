namespace Common.DaprActor

module ActorResult =

  open System.Threading.Tasks

  type ActorOptionResult<'a> = { Data: 'a option }

  let toActorOptionResult<'a> (t: Task<'a option>) =
    task {
      let! result = t
      let result = { Data = result }
      return result
    }
