module MarkdownCyoa.Core.Parser.Document
open Farkdown.SyntaxTree

let parse str =
    let markdownDocument = Document.parse str
    markdownDocument
    |> Result.map (fun statements ->
        statements
        |> List.choose (fun statement ->
            match statement with
            | Statement.Header header ->
                Paragraph.parse header
            | _ ->
                None
        )
    )
