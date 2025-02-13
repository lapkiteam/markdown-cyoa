module MarkdownCyoa.Core.Parser.Action
open Farkdown.SyntaxTree

open MarkdownCyoa.Core

let parse ((line, body): ListItem) =
    match line with
    | [LineElement.Link link] ->
        Some {
            Reference =
                let href = link.Href
                if href.Length <= 0 then
                    href
                else
                    match href[0] with
                    | '#' -> href[1..]
                    | _ -> href
            Description = link.Description
        }
    | _ ->
        None
