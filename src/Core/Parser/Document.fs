module MarkdownCyoa.Core.Parser.Document

let parse str : Result<MarkdownCyoa.Core.Document, _> =
    Farkdown.SyntaxTree.Document.parse str
    |> Result.map (fun statements ->
        statements
        |> List.choose (fun statement ->
            match statement with
            | Farkdown.SyntaxTree.Statement.Header header ->
                Some (Scene.parse header)
            | _ ->
                None
        )
    )
