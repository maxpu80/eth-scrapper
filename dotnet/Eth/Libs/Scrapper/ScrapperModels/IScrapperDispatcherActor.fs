namespace ScrapperModels

open Dapr.Actors
open System.Threading.Tasks

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

type State =
  { Status: Status
    Request: ScrapperRequest }

type IScrapperDispatcherActor =
  inherit IActor
  abstract Start: data: StartData -> Task<bool>
  abstract Continue: data: ContinueData -> Task<bool>
  abstract Pause: unit -> Task<bool>
  abstract Resume: unit -> Task<bool>
  abstract State: unit -> Task<State option>
  abstract Reset: unit -> Task<bool>
