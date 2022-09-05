namespace ScrapperModels

type EventData =
  { Event: string
    Block: uint
    Data: Map<string, obj> }

type BlockRange = { From: uint; To: uint }

type Success =
  { Events: EventData list
    BlockRange: BlockRange }

type ErrorData =
  | EmptyResult
  | LimitExceeded
  | Unknown

type Error =
  { Data: ErrorData
    BlockRange: BlockRange }

type Result = Result<Success, Error>

type RequestBlockRange = { From: uint option; To: uint option }
