module MarkdownCyoa.Core.Qsp
open Qsp.Ast

module LineKind =
    open Farkdown.SyntaxTree

    let ofFarkdownLineElement (lineElement: Farkdown.SyntaxTree.LineElement) : LineKind =
        match lineElement with
        | LineElement.Text text ->
            LineKind.StringKind text
        | LineElement.Bold str ->
            LineKind.StringKind $"<b>{str}</b>"
        | LineElement.Italic str ->
            LineKind.StringKind $"<i>{str}</i>"
        | LineElement.Strikeout str ->
            LineKind.StringKind $"<del>{str}</del>"
        | LineElement.Underline str ->
            LineKind.StringKind $"<u>{str}</u>"
        | LineElement.Comment comment ->
            LineKind.StringKind $"<!-- {comment} -->"
        | LineElement.Link link ->
            // todo: feat(core/qsp): make full a link description converter
            let description: string =
                match link.Description with
                | [LineElement.Text text] -> text
                | _ -> ""
            LineKind.StringKind
                $"<a href=\"{link.Href}\" title=\"{link.Title}\">{description}</a>"
        | LineElement.Image image ->
            LineKind.StringKind
                $"<img src=\"{image.Src}\" alt=\"{image.Alt}\" title=\"{image.Title}\" />"

module Line =
    let ofFarkdownLine (line: Farkdown.SyntaxTree.Line) : Line =
        line
        |> List.map LineKind.ofFarkdownLineElement

module Expr =
    let createSimpleString str =
        Expr.Val (Value.String [
            [LineKind.StringKind str]
        ])

    let ofFarkdownLine (line: Farkdown.SyntaxTree.Line) =
        Expr.Val (
            Value.String (
                [Line.ofFarkdownLine line]
            )
        )

module PosStatement =
    let createSimple statement : PosStatement =
        new NoEqualityPosition(Position.empty), statement

module Statement =
    open Farkdown.SyntaxTree

    let rec ofFarkdownStatement (statement: Farkdown.SyntaxTree.Statement): Qsp.Ast.Statements =
        match statement with
        | Statement.Header h ->
            [
                PosStatement.createSimple (
                    Qsp.Ast.Statement.Proc("*pl", [
                        Expr.createSimpleString $"<h{h.Level}>{h.Title}</h{h.Level}>"
                    ])
                )
                yield! List.collect ofFarkdownStatement h.Body
            ]
        | Statement.Paragraph p ->
            [
                PosStatement.createSimple (
                    Qsp.Ast.Statement.Proc("*pl", [
                        Expr.Val (Value.String (
                            List.map Line.ofFarkdownLine p
                        ))
                    ])
                )
                PosStatement.createSimple (Qsp.Ast.Statement.Proc("*pl", [
                    Expr.createSimpleString ""
                ]))
            ]
        | Statement.Comment comment ->
            [
                PosStatement.createSimple (
                    Qsp.Ast.Statement.Comment comment
                )
            ]
        | Statement.List(isOrdered, items) ->
            [] // todo: feat:

module Main =
    let toQspStatements (main: Main): Qsp.Ast.Statements =
        List.collect Statement.ofFarkdownStatement main

module Action =
    let toQspAct (action: Action) =
        Statement.Act(
            [Expr.ofFarkdownLine action.Description], [
                new NoEqualityPosition(Position.empty), Statement.Proc("gt", [
                    Expr.createSimpleString action.Reference
                ])
            ]
        )

let toQsp (markdownCyoaDocument: MarkdownCyoa.Core.Document): Document =
    markdownCyoaDocument
    |> List.map (fun paragraph ->
        let body: Statements =
            Main.toQspStatements paragraph.Main
        let actions: Statements =
            paragraph.Actions
            |> List.map (fun action ->
                new NoEqualityPosition(Position.empty), Action.toQspAct action
            )
        DocumentElement.Location(Location.Location (paragraph.Id, [
            yield! body
            yield! actions
        ]))
    )
