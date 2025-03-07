module MarkdownCyoa.Core.Qsp
open Qsp.Ast

module LineKind =
    open Farkdown.SyntaxTree
    open Qsp.Printer.Ast

    let ofFarkdownLineElement ofFarkdownLine (lineElement: Farkdown.SyntaxTree.LineElement) : LineKind =
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
            let description =
                let showStmtsInline posStatements =
                    FsharpMyExtension.ShowList.empty // todo
                let showExpr =
                    Expr.Printer.showExpr showStmtsInline
                let lines = ofFarkdownLine link.Description |> List.singleton
                Value.Printer.showStringLines showExpr showStmtsInline lines
                |> FsharpMyExtension.ShowList.lines
                |> FsharpMyExtension.ShowList.show
            LineKind.StringKind
                $"<a href=\"{link.Href}\" title=\"{link.Title}\">{description}</a>"
        | LineElement.Image image ->
            LineKind.StringKind
                $"<img src=\"{image.Src}\" alt=\"{image.Alt}\" title=\"{image.Title}\" />"

module Line =
    let rec ofFarkdownLine (line: Farkdown.SyntaxTree.Line) : Line =
        line
        |> List.map (LineKind.ofFarkdownLineElement ofFarkdownLine)

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
    let toQspAct (optionalSceneId: ParagraphId option) (action: Action) =
        Statement.Act(
            [Expr.ofFarkdownLine action.Description], [
                PosStatement.createSimple (Statement.Proc("gt", [
                    Expr.Var (VarType.StringType, "curLoc")

                    match optionalSceneId with
                    | Some sceneId -> Expr.createSimpleString sceneId
                    | None -> ()

                    Expr.createSimpleString action.Reference
                ]))
            ]
        )

module Paragraph =
    let toQsp optionalSceneId (paragraph: Paragraph) =
        let body: Statements =
            Main.toQspStatements paragraph.Main
        let actions: Statements =
            paragraph.Actions
            |> List.map (fun action ->
                PosStatement.createSimple (Action.toQspAct optionalSceneId action)
            )
        [
            yield! body
            yield! actions
        ]

module Scene =
    let toQsp (scene: Scene) =
        List.foldBack
            (fun (paragraph: Paragraph) state ->
                let expr =
                    // $args[1] = paragraph.Id
                    Expr.Expr(
                        Op.Eq,
                        Expr.Arr(
                            (VarType.StringType, "args"),
                            [Expr.Val (Value.Int 1)]
                        ),
                        Expr.createSimpleString paragraph.Id
                    )
                [PosStatement.createSimple (Qsp.Ast.If(
                    expr,
                    Paragraph.toQsp (Some scene.Id) paragraph,
                    state
                ))]
            )
            scene.Body
            []

module Location =
    let toQsp (location: MarkdownCyoa.Core.Location) =
        let body =
            List.foldBack
                (fun (scene: Scene) state ->
                    let expr =
                        // $args[0] = scene.Id
                        Expr.Expr(
                            Op.Eq,
                            Expr.Arr(
                                (VarType.StringType, "args"),
                                [Expr.Val (Value.Int 0)]
                            ),
                            Expr.createSimpleString scene.Id
                        )
                    [PosStatement.createSimple (Qsp.Ast.If(
                        expr,
                        Scene.toQsp scene,
                        state
                    ))]
                )
                location.Body
                []

        DocumentElement.Location(
            Location.Location (
                location.Id, body
            )
        )

let toQsp (markdownCyoaDocument: MarkdownCyoa.Core.Document): Document =
    markdownCyoaDocument
    |> List.map Location.toQsp
