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

type Document = Paragraph list
