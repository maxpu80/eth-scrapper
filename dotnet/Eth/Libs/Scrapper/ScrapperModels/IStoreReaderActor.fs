namespace ScrapperModels

open System.Text.Json.Serialization
open Dapr.Actors
open System.Threading.Tasks
open System.Text.Json

type StoreReaderData = { Marker: string }

type IStoreReaderActor =
  inherit IActor
  abstract Read: data: StoreReaderData -> Task<bool>
