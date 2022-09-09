namespace Common.Dapr

[<RequireQualifiedAccess>]
module ActorLogging =
  open Dapr.Actors.Runtime

  let create (host: ActorHost) =
    host.LoggerFactory.CreateLogger($"{host.ActorTypeInfo.ActorTypeName}::{host.Id}")
