namespace ScrapperDispatcherActor

[<AutoOpen>]
module ScrapperDispatcherActor =

  open Dapr.Actors
  open Dapr.Actors.Runtime
  open System.Threading.Tasks
  open ScrapperModels

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

  [<Actor(TypeName = "scrapper-dispatcher")>]
  type ScrapperDispatcherActor(host: ActorHost) =
    inherit Actor(host)

    interface IScrapperDispatcherActor with
      member this.TestStart data =
        task {
          let actor =
            this.ProxyFactory.CreateActorProxy<IScrapperStoreActor>(this.Id, "scrapper-store")

          let data: ContinueData =
            { ContractAddress = "test"
              Abi = "test"
              Result =
                { Events =
                    [ { Event = "test"
                        Block = 1u
                        Data = "test" } ]
                  RequestBlockRange = { From = None; To = None }
                  BlockRange = { From = 0u; To = 0u } }
                |> Ok }

          actor.Test data |> ignore
          return true
        }

      member this.Start data =
        task {

          let scrapperRequest: ScrapperRequest =
            { ContractAddress = data.ContractAddress
              Abi = data.Abi
              BlockRange = { From = None; To = None } }

          do! runScrapper this.ProxyFactory this.Id scrapperRequest

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
