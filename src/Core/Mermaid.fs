module MarkdownCyoa.Core.Mermaid
open FMermaid.Api

let toMermaid (markdownCyoaDocument: Document): Flowchart.Document =
    markdownCyoaDocument
    |> List.collect (fun paragraph ->
        paragraph.Actions
        |> List.map (fun action ->
            {
                Flowchart.FromNode = paragraph.Id
                Flowchart.EdgeLabel =
                    Farkdown.SyntaxTree.Line.Show.show action.Description
                    |> FsharpMyExtension.ShowList.show
                    |> Some
                Flowchart.ToNode = action.Reference
            }
            |> Flowchart.Statement.Connection
        )
    )
    |> Flowchart.Document.create
