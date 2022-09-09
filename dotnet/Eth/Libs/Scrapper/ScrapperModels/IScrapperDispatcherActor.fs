namespace ScrapperModels

open Dapr.Actors
open System.Threading.Tasks
open Common.DaprActor.ActorResult

type StartData =
  { ContractAddress: string
    Abi: string }

type ContinueData =
  { ContractAddress: string
    Abi: string
    Result: Result }

[<RequireQualifiedAccess>]
type Status =
  | Continue
  | Pause
  | Finish

type State =
  { Status: Status
    Request: ScrapperRequest
    Date: int64
    FinishDate: int64 option }

type IScrapperDispatcherActor =
  inherit IActor
  abstract Start: data: StartData -> Task<bool>
  abstract Continue: data: ContinueData -> Task<bool>
  abstract Pause: unit -> Task<bool>
  abstract Resume: unit -> Task<bool>
  abstract State: unit -> Task<ActorOptionResult<State>>
  abstract Reset: unit -> Task<bool>
