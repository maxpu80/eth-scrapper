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

type IScrapperDispatcherActor =
  inherit IActor
  abstract TestStart: data: unit -> Task<bool>
  abstract Start: data: StartData -> Task<bool>
  abstract Continue: data: ContinueData -> Task<bool>
