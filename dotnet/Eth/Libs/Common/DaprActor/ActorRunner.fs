namespace Common.DaprActor

[<AutoOpen>]
module ActorRunner =
  open Dapr.Actors
  open Dapr.Actors.Runtime

  let invokeActor<'a, 'r>
    (proxyFactory: Client.IActorProxyFactory)
    actorId
    (actorType: string)
    (methodName: string)
    (data: 'a)
    =
    task {

      let actor = proxyFactory.Create(actorId, actorType)

      actor.InvokeMethodAsync<'a, 'r>(methodName, data)
      |> ignore
    }

  let invokeActorId<'a, 'r> (actorHost: ActorHost) actorType methodName (data: 'a) =
    invokeActor actorHost.ProxyFactory actorHost.Id actorType methodName data
