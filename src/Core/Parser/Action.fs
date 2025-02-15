module MarkdownCyoa.Core.Parser.Action
open Farkdown.SyntaxTree

open MarkdownCyoa.Core

module Farkdown =
    module SyntaxTree =
        module LineElement =
            let rec tryPickLink = function
                | LineElement.Link link ->
                    Some link
                | LineElement.Bold body
                | LineElement.Italic body
                | LineElement.Strikeout body
                | LineElement.Underline body ->
                    tryPickLink body
                | LineElement.Text(_)
                | LineElement.Comment(_)
                | LineElement.Image(_) ->
                    None

        module Line =
            let tryPickLink (line: Line) =
                line
                |> List.tryPick (fun lineElement ->
                    LineElement.tryPickLink lineElement
                )

open Farkdown.SyntaxTree

let parse ((line, body): ListItem) =
    Line.tryPickLink line
    |> Option.map (fun link ->
        {
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
    )
