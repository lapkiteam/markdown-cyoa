module MarkdownCyoa.Core
open Farkdown.SyntaxTree

type ParagraphId = string

type Action =
    {
        Reference: ParagraphId
        Description: Line
    }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Action =
    module Parser =
        let parser ((line, body): ListItem) =
            match line with
            | [LineElement.Link link] ->
                Some {
                    Reference = link.Href
                    Description = link.Description
                }
            | _ ->
                None

type Paragraph =
    {
        Id: ParagraphId
        Title: Line
        Main: Statement list
        Actions: Action list
    }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Paragraph =
    module Parser =
        let parser (header: Header) =
            match List.rev header.Body with
            | x::xs ->
                Some {
                    Id = "" // todo: extract from header.Title
                    Title = header.Title
                    Main = List.rev xs
                    Actions =
                        match x with
                        | Statement.List(_, items) ->
                            items
                            |> List.choose (fun listItem ->
                                Action.Parser.parser listItem
                            )
                        | _ -> []
                }
            | [] ->
                None

type Document = Paragraph list

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Document =
    module Parser =
        let parse str =
            let markdownDocument = Document.parse str
            markdownDocument
            |> Result.map (fun statements ->
                statements
                |> List.choose (fun statement ->
                    match statement with
                    | Statement.Header header ->
                        Paragraph.Parser.parser header
                    | _ ->
                        None
                )
            )
