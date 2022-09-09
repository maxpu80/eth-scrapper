namespace ScrapperDispatcherActor

#nowarn "20"

module Program =

  [<EntryPoint>]
  let main args =
    Common.Dapr.DaprActor.createDaprActor (fun opts -> opts.Actors.RegisterActor<ScrapperDispatcherActor>()) args
