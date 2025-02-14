module MarkdownCyoa.Core.Parser.Scene
open MarkdownCyoa.Core

let parse (farkdownHeader: Farkdown.SyntaxTree.Header) : Scene =
    {
        Id = ParagraphId.parse farkdownHeader.Title
        Title = farkdownHeader.Title
        Body =
            farkdownHeader.Body
            |> List.choose (fun statement ->
                match statement with
                | Farkdown.SyntaxTree.Statement.Header header ->
                    Paragraph.parse header
                | _ ->
                    None
            )
    }
