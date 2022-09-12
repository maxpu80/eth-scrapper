namespace ScrapperModels

open Dapr.Actors
open System.Threading.Tasks
open Common.DaprActor.ActorResult
open System.Runtime.Serialization
open System.Reflection
open Microsoft.FSharp.Reflection

type StartData =
  { EthProviderUrl: string
    ContractAddress: string
    Abi: string }

type ContinueData =
  { EthProviderUrl: string
    ContractAddress: string
    Abi: string
    Result: Result }

[<RequireQualifiedAccess>]
type Status =
  | Continue
  | Pause
  | Finish
  | Schedule

type State =
  { Status: Status
    Request: ScrapperRequest
    Date: int64
    FinishDate: int64 option }


[<KnownType("KnownTypes")>]
type ScrapperDispatcherActorError =
  | StateConflict of State * string
  | StateNotFound
  static member KnownTypes() =
    knownTypes<ScrapperDispatcherActorError> ()


type ScrapperDispatcherActorResult = Result<State, ScrapperDispatcherActorError>


type IScrapperDispatcherActor =
  inherit IActor
  abstract Start: data: StartData -> Task<ScrapperDispatcherActorResult>
  abstract Continue: data: ContinueData -> Task<ScrapperDispatcherActorResult>
  abstract Pause: unit -> Task<ScrapperDispatcherActorResult>
  abstract Resume: unit -> Task<ScrapperDispatcherActorResult>
  abstract State: unit -> Task<State option>
  abstract Reset: unit -> Task<bool>
  abstract Schedule: unit -> Task<ScrapperDispatcherActorResult>
