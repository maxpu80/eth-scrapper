namespace ScrapperDispatcher

[<AutoOpen>]
module NextBlockRangeCalc =

  let private successNextBlockRangeCalc (result: Success) =

    // Take from current "To" block till "latest"
    let resultRange =
      { From = Some result.BlockRange.To
        To = None }

    resultRange

  let private errorNextBlockRangeCalc (error: Error) =
    match error.Data with
    | LimitExceeded ->
      // Decrease range by half
      { From = Some error.BlockRange.From
        To = Some(error.BlockRange.To / 2u) }
    | EmptyResult ->
      // Increase by 2
      { From = Some error.BlockRange.From
        To = Some(error.BlockRange.To * 2u) }
    | Unknown ->
      // Left as it is
      { From = Some error.BlockRange.From
        To = Some error.BlockRange.To }

  let nextBlockRangeCalc: NextBlockRangeCalcFun =
    function
    | Ok success -> successNextBlockRangeCalc success
    | Error error -> errorNextBlockRangeCalc error
