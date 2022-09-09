namespace Common.DaprActor

[<AutoOpen>]
module ActorState =
  open Dapr.Actors.Runtime

  let getState<'a> (name: string) (stateManager: IActorStateManager) () =
    task {
      let! result = stateManager.TryGetStateAsync<'a> name

      return
        match result.HasValue with
        | true -> Some result.Value
        | false -> None
    }

  let setState<'a> (name: string) (stateManager: IActorStateManager) (state: 'a) =
    stateManager.SetStateAsync(name, state)

  let stateManager<'a> (name: string) (stateManager: IActorStateManager) =
    {| Get = getState<'a> name stateManager
       Set = setState<'a> name stateManager |}
