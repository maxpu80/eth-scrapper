namespace ScrapperDispatcherActor

[<AutoOpen>]
module ScrapperDispatcherActor =

  open Dapr.Actors
  open Dapr.Actors.Runtime
  open System.Threading.Tasks
  open ScrapperModels
  open Microsoft.Extensions.Logging
  open Common.DaprActor

  type BlockRangeDTO =
    { From: System.Nullable<uint>
      To: System.Nullable<uint> }

  type ScrapperRequestDTO =
    { ContractAddress: string
      Abi: string
      BlockRange: BlockRangeDTO }

  let private toDTO (request: ScrapperRequest) : ScrapperRequestDTO =
    { ContractAddress = request.ContractAddress
      Abi = request.Abi
      BlockRange =
        { From =
            match request.BlockRange.From with
            | Some from -> System.Nullable(from)
            | None -> System.Nullable()
          To =
            match request.BlockRange.To with
            | Some _to -> System.Nullable(_to)
            | None -> System.Nullable() } }

  let private checkStop (result: Result) =
    match result with
    // read successfully till the latest block
    | Ok success when success.RequestBlockRange.To = None -> true
    | Error error ->
      match error.Data with
      // read successfully till the latest block
      | EmptyResult when error.RequestBlockRange.To = None -> true
      | _ -> false
    | _ -> false


  let private runScrapper (proxyFactory: Client.IActorProxyFactory) actorId scrapperRequest =
    task {

      let actor = proxyFactory.Create(actorId, "ScrapperActor")

      let dto = scrapperRequest |> toDTO

      actor.InvokeMethodAsync<ScrapperRequestDTO, Result>("scrap", dto)
      |> ignore
    }

  let private STATE_NAME = "state"

  [<Actor(TypeName = "scrapper-dispatcher")>]
  type ScrapperDispatcherActor(host: ActorHost) as this =
    inherit Actor(host)
    let logger = ActorLogging.create host
    let stateManager = stateManager<State> STATE_NAME this.StateManager

    interface IScrapperDispatcherActor with
      member this.Start data =

        logger.LogDebug("Start with {@data}", data)

        task {

          let! state = stateManager.Get()

          match state with
          | Some _ ->
            logger.LogError("Try to start version which already started", data)
            return false
          | None ->
            let scrapperRequest: ScrapperRequest =
              { ContractAddress = data.ContractAddress
                Abi = data.Abi
                BlockRange = { From = None; To = None } }

            logger.LogDebug("Run scrapper with @{data}", scrapperRequest)

            //do! runScrapper this.ProxyFactory this.Id scrapperRequest

            let state: State =
              { Status = Status.Continue
                Request = scrapperRequest }

            do! stateManager.Set state

            return true
        }

      member this.Continue data =
        match checkStop data.Result with
        | false ->
          task {
            let blockRange = nextBlockRangeCalc data.Result

            let scrapperRequest: ScrapperRequest =
              { ContractAddress = data.ContractAddress
                Abi = data.Abi
                BlockRange = blockRange }

            do! runScrapper this.ProxyFactory this.Id scrapperRequest

            return true
          }
        | true -> task { return false }


      member this.Pause() = task { return true }
      member this.Resume() = task { return true }
      member this.State() = task { return None }
      member this.Reset() = task { return true }
