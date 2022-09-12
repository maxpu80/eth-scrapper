namespace ScrapperModels

[<AutoOpen>]
module internal Helpers =
  open System.Reflection
  open Microsoft.FSharp.Reflection
  let knownTypes<'a> () =
    typeof<'a>.GetNestedTypes (BindingFlags.Public ||| BindingFlags.NonPublic)
    |> Array.filter FSharpType.IsUnion
