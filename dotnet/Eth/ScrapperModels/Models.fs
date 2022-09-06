namespace ScrapperModels

type EventData =
  { Event: string
    Block: uint
    Data: Map<string, obj> }

type BlockRange = { From: uint; To: uint }

type RequestBlockRange = { From: uint option; To: uint option }

type Success =
  { Events: EventData list
    RequestBlockRange : RequestBlockRange
    BlockRange: BlockRange }

type ErrorData =
  | EmptyResult
  | LimitExceeded
  | Unknown

type Error =
  { Data: ErrorData
    RequestBlockRange : RequestBlockRange
    BlockRange: BlockRange }

type Result = Result<Success, Error>
