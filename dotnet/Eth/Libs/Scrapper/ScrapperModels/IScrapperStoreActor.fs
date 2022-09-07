namespace ScrapperModels

open System.Text.Json.Serialization
open Dapr.Actors
open System.Threading.Tasks
open System.Text.Json

type ContinueSuccessData =
  { ContractAddress: string
    Abi: string
    Result: Success }

type TestData =
  { ContractAddress: string
    Abi: string
    Result: Success}

type IScrapperStoreActor =
  inherit IActor
  abstract Store: data: ContinueSuccessData -> Task<bool>
  abstract Test: data: unit -> Task<bool>
