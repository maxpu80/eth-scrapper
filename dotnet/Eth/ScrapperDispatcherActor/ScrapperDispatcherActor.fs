namespace ScrapperDispatcherActor

[<AutoOpen>]
module ScrapperDispatcherActor =

  open Dapr.Actors
  open Dapr.Actors.Runtime
  open System.Threading.Tasks
  open ScrapperModels

  type StartData =
    { ContractAddress: string
      Abi: string }

  type ContinueData =
    { ContractAddress: string
      Abi: string
      Result: Result }

  type Error = | Unknown

  type BlockRangeDTO =
    { From: System.Nullable<uint>
      To: System.Nullable<uint> }

  type ScrapperRequestDTO =
    { ContractAddress: string
      Abi: string
      BlockRange: BlockRangeDTO }

  let toDTO (request: ScrapperRequest) : ScrapperRequestDTO =
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
            | None -> System.Nullable() }

    }

  [<Actor(TypeName = "scrapper-dispatcher")>]
  type ScrapperDispatcherActor(host: ActorHost) =
    inherit Actor(host)

    interface IScrapperDispatcherActor with
      member this.Start data =
        task {
          let actorId = this.Id

          let actor = this.ProxyFactory.Create(actorId, "ScrapperActor")


          let scrapperRequest: ScrapperRequest =
            { ContractAddress = data.ContractAddress
              Abi = data.Abi
              BlockRange = { From = None; To = None } }

          let dto = scrapperRequest |> toDTO

          actor.InvokeMethodAsync<ScrapperRequestDTO, Result>("scrap", dto)
          |> ignore

          return true
        }

      member this.Continue data =

        task {
          let actorId = this.Id

          let actor = this.ProxyFactory.Create(actorId, "ScrapperActor")

          let blockRange = nextBlockRangeCalc data.Result

          let scrapperRequest: ScrapperRequest =
            { ContractAddress = data.ContractAddress
              Abi = data.Abi
              BlockRange = blockRange }

          let dto = scrapperRequest |> toDTO

          actor.InvokeMethodAsync<ScrapperRequestDTO, Result>("scrap", dto)
          |> ignore

          return true
        }
