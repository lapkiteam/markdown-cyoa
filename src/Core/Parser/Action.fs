module MarkdownCyoa.Core.Parser.Action
open Farkdown.SyntaxTree

open MarkdownCyoa.Core

let parse ((line, body): ListItem) =
    match line with
    | [LineElement.Link link] ->
        Some {
            Reference = link.Href
            Description = link.Description
        }
    | _ ->
        None
