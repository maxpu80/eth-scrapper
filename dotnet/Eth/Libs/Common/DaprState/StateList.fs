namespace Common.DaprState

[<AutoOpen>]
module StateList =

  open State

  let updateOrCreateStateList<'a> env id (entity: 'a) =
    tryUpdateOrCreateStateAsync env id (fun _ -> [ entity ]) (fun data -> entity :: data)


  let getStateList<'a> env id =
    task {
      let! result = getStateAsync<'a list> env id

      return
        match result with
        | Some list -> list
        | None -> []
    }

  let getStateListHead<'a> env id fn =
    task {
      let! result = getStateAsync<'a list> env id

      return
        match result with
        | Some list -> list |> List.filter fn |> List.tryHead
        | None -> None
    }

  let private toOptList =
    function
    | [] -> None
    | _ as x -> Some x

  let deleteStateList<'a when 'a: equality> env id (fr: 'a -> bool) =
    task {
      let! result = tryUpdateStateAsync' env id (List.filter (fr >> not) >> Some)

      match result with
      | Some (cur, orig) ->
        let res = List.except cur orig
        return res |> List.tryHead
      | None -> return None
    }

  let updateStateList<'a> env id (fr: 'a -> bool) mp =
    task {
      let! result =
        tryUpdateStateAsync'
          env
          id
          (List.map (fun x -> if fr x then mp x else x)
           >> toOptList)

      match result with
      | Some (cur, _) -> return cur |> List.filter (fr) |> List.tryHead
      | None -> return None
    }


  let stateListRepo<'a when 'a: equality> env =
    {| Set = updateOrCreateStateList<'a> env
       GetAll = getStateList<'a> env
       GetHead = getStateListHead<'a> env
       Delete = deleteStateList<'a> env
       Update = updateStateList<'a> env |}
