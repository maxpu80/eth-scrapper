namespace ScrapperModels

open System.Runtime.Serialization

type BlockRange = { From: uint; To: uint }

type RequestBlockRange = { From: uint option; To: uint option }

type Success =
  { RequestBlockRange: RequestBlockRange
    BlockRange: BlockRange }

type ErrorData =
  | EmptyResult
  | LimitExceeded
  | Unknown

type Error =
  { Data: ErrorData
    RequestBlockRange: RequestBlockRange
    BlockRange: BlockRange }

type Result = Result<Success, Error>

type ScrapperRequest =
  { ContractAddress: string
    Abi: string
    BlockRange: RequestBlockRange }
