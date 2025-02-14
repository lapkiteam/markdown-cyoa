namespace MarkdownCyoa.Core
open Farkdown.SyntaxTree

type ParagraphId = string

type Action =
    {
        Reference: ParagraphId
        Description: Line
    }

type Main = Statement list

type Paragraph =
    {
        Id: ParagraphId
        Title: Line
        Main: Statement list
        Actions: Action list
    }

type Scene =
    {
        Id: string
        Title: Line
        Body: Paragraph list
    }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Scene =
    let create id title body =
        {
            Id = id
            Title = title
            Body = body
        }

type Document = Scene list
