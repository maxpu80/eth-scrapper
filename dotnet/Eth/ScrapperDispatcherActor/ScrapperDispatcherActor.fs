namespace ScrapperDispatcherActor

[<AutoOpen>]
module ScrapperDispatcherActor =

  open Dapr.Actors
  open Dapr.Actors.Runtime
  open System.Threading.Tasks
  open ScrapperModels
  open Microsoft.Extensions.Logging
  open Common.DaprActor
  open System

  type BlockRangeDTO =
    { From: System.Nullable<uint>
      To: System.Nullable<uint> }

  type ScrapperRequestDTO =
    { EthProviderUrl: string
      ContractAddress: string
      Abi: string
      BlockRange: BlockRangeDTO }

  let private toDTO (request: ScrapperRequest) : ScrapperRequestDTO =
    { EthProviderUrl = request.EthProviderUrl
      ContractAddress = request.ContractAddress
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
    let dto = scrapperRequest |> toDTO

    invokeActor<ScrapperRequestDTO, Result> proxyFactory actorId "ScrapperActor" "scrap" dto


  let private STATE_NAME = "state"
  let private SCHEDULE_TIMER_NAME = "timer"

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
              { EthProviderUrl = data.EthProviderUrl
                ContractAddress = data.ContractAddress
                Abi = data.Abi
                BlockRange = { From = None; To = None } }

            logger.LogDebug("Run scrapper with @{data}", scrapperRequest)

            do! runScrapper this.ProxyFactory this.Id scrapperRequest

            let state: State =
              { Status = Status.Continue
                Request = scrapperRequest
                Date = epoch ()
                FinishDate = None }

            do! stateManager.Set state

            return true
        }

      member this.Continue data =
        logger.LogDebug("Continue with {@data}", data)
        let me = this :> IScrapperDispatcherActor

        task {
          let! state = stateManager.Get()

          match state with
          | Some state when state.Status = Status.Pause ->
            logger.LogWarning("Actor in paused state, skip continue")
            return false
          | _ ->
            let blockRange = nextBlockRangeCalc data.Result

            let scrapperRequest: ScrapperRequest =
              { EthProviderUrl = data.EthProviderUrl
                ContractAddress = data.ContractAddress
                Abi = data.Abi
                BlockRange = blockRange }

            match checkStop data.Result with
            | false ->

              logger.LogDebug("Stop check is false, continue", scrapperRequest)

              let finishDate =
                match state with
                | Some state -> state.FinishDate
                | None ->
                  logger.LogWarning("Continue, but state not found")
                  None

              do! runScrapper this.ProxyFactory this.Id scrapperRequest

              let state: State =
                { Status = Status.Continue
                  Request = scrapperRequest
                  Date = epoch ()
                  FinishDate = finishDate }

              do! stateManager.Set state

              return true

            | true ->

              logger.LogInformation("Stop check is true, finish")

              let state: State =
                { Status = Status.Finish
                  Request = scrapperRequest
                  Date = epoch ()
                  FinishDate = epoch () |> Some }

              do! stateManager.Set state

              let! _ = me.Schedule()

              return false
        }


      member this.Pause() =
        task {
          let! state = stateManager.Get()

          match state with
          | Some state when
            state.Status = Status.Continue
            || state.Status = Status.Schedule
            ->
            let state =
              { state with
                  Status = Status.Pause
                  Date = epoch () }

            do! stateManager.Set state
            return true
          | _ -> return false

        }

      member this.Resume() =
        task {
          let! state = stateManager.Get()

          match state with
          | Some state when
            state.Status = Status.Pause
            || state.Status = Status.Finish
            ->

            let updatedState =
              { state with
                  Status = Status.Continue
                  Date = epoch () }

            logger.LogInformation("Resume with {@pervState} {@state}", state, updatedState)

            do! runScrapper this.ProxyFactory this.Id updatedState.Request

            do! stateManager.Set updatedState
            return true
          | _ ->
            logger.LogWarning("Resume state not found or Continue")
            return false

        }

      member this.State() =
        stateManager.Get()
        |> ActorResult.toActorOptionResult

      member this.Reset() = stateManager.Remove()

      member this.Schedule() =
        let dueTime = 60.
        logger.LogInformation("Try schedule {dueTime}", dueTime)

        task {
          let! state = stateManager.Get()

          match state with
          | Some state when state.Status = Status.Finish ->
            logger.LogInformation("Run schedule with {@state}", state)

            let updatedState =
              { state with
                  Status = Status.Schedule
                  Date = epoch () }

            do! stateManager.Set updatedState

            let! _ =
              this.RegisterTimerAsync(SCHEDULE_TIMER_NAME, "Resume", [||], TimeSpan.FromSeconds(dueTime), TimeSpan.Zero)

            return true
          | _ as state ->
            logger.LogWarning("Can't schedule, wrong state {@state}", state)
            return false
        }
