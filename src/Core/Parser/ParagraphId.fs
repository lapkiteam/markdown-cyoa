module MarkdownCyoa.Core.Parser.ParagraphId
open Farkdown.SyntaxTree

let parse (headerTitle: Line) =
    headerTitle
    |> List.map (
        let prepareString (str: string) =
            str
            |> String.collect (function
                | ' ' -> "-"
                | ',' -> ""
                | c ->
                    sprintf "%c" (System.Char.ToLower c)
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
