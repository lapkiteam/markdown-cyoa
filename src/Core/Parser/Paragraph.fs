module MarkdownCyoa.Core.Parser.Paragraph
open Farkdown.SyntaxTree

open MarkdownCyoa.Core

let parse (header: Header) =
    match List.rev header.Body with
    | x::xs ->
        Some {
            Id =
                header.Title
                |> List.map (
                    let prepareString (str: string) =
                        str
                        |> String.map (function
                            | ' ' -> '-'
                            | c -> System.Char.ToLower c
                        )
                    let rec linear = function
                        | LineElement.Text text ->
                            prepareString text
                        | LineElement.Bold lineElement
                        | LineElement.Italic lineElement
                        | LineElement.Strikeout lineElement
                        | LineElement.Underline lineElement ->
                            linear lineElement
                        | LineElement.Comment _ -> ""
                        | LineElement.Link link ->
                            "" // todo spacesToHyphens x.Description
                        | LineElement.Image image ->
                            image.Alt
                    linear
                )
                |> String.concat ""
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
