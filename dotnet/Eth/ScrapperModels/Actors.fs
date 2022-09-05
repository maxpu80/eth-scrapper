namespace ScrapperModels

open System.Text.Json.Serialization
open Dapr.Actors
open System.Threading.Tasks
open System.Text.Json

type ScrapperRequest =
  { ContractAddress: string
    Abi: string
    BlockRange: RequestBlockRange }

type IScrapperActor =
  inherit IActor
  abstract Scrap: data: ScrapperRequest -> Task<Result>

type StartData =
  { ContractAddress: string
    Abi: string }

type ContinueData =
  { ContractAddress: string
    Abi: string
    Result: Result }

type IScrapperDispatcherActor =
  inherit IActor
  abstract Start: data: StartData -> Task<bool>
  abstract Continue: data: ContinueData -> Task<bool>
