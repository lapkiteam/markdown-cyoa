module MarkdownCyoa.Core.Parser.Paragraph
open Farkdown.SyntaxTree

open MarkdownCyoa.Core

let parse (header: Header) =
    match List.rev header.Body with
    | x::xs ->
        Some {
            Id = "" // todo: extract from header.Title
            Title = header.Title
            Main = List.rev xs
            Actions =
                match x with
                | Statement.List(_, items) ->
                    items
                    |> List.choose (fun listItem ->
                        Action.parse listItem
                    )
                | _ -> []
        }
    | [] ->
        None
