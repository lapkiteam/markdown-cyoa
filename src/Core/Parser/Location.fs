module MarkdownCyoa.Core.Parser.Location
open Farkdown.SyntaxTree

open MarkdownCyoa.Core

let parse (header: Header) : Location =
    Location.create
        (ParagraphId.parse header.Title)
        header.Title
        (
            header.Body
            |> List.choose (fun statement ->
                match statement with
                | Statement.Header header ->
                    Some (Scene.parse header)
                | _ ->
                    None
            )
        )
