﻿module MarkdownCyoa.Cli
open MarkdownCyoa.Core
open MarkdownCyoa.Core.Parser
open FMermaid.Api
open Argu
open FsharpMyExtension.ShowList

[<RequireQualifiedAccess>]
type ToMermaidCliArguments =
    | [<AltCommandLine("-c")>] Clipboard
    | [<MainCommand; ExactlyOnce; Last>] Source_Path of path: string
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Source_Path _ -> "path to a markdown cyoa document"
            | Clipboard -> "output to clipboard"

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module ToMermaidCliArguments =
    let exec (results: ParseResults<ToMermaidCliArguments>) =
        let sourcePath = results.GetResult ToMermaidCliArguments.Source_Path
        let source =
            match sourcePath with
            | "-" -> Clipboard.getText()
            | _ -> System.IO.File.ReadAllText sourcePath
        Document.parse source
        |> Result.map (fun markdownCyoa ->
            Mermaid.toMermaid markdownCyoa
            |> Flowchart.Document.Show.shows (
                showString "  "
            )
            |> joinEmpty "\n"
            |> show
        )
        |> function
            | Ok str ->
                match results.TryGetResult ToMermaidCliArguments.Clipboard with
                | Some _ ->
                    Clipboard.setText str
                | None ->
                    printfn "%s" str
                0
            | Error errMsg ->
                eprintfn "%s" errMsg
                1

[<RequireQualifiedAccess>]
type ToQspCliArguments =
    | [<AltCommandLine("-c")>] Clipboard
    | [<MainCommand; ExactlyOnce; Last>] Source_Path of path: string
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Source_Path _ -> "path to a markdown cyoa document or - from stdin"
            | Clipboard -> "output to clipboard"

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module ToQspCliArguments =
    open Qsp.Printer

    let exec (results: ParseResults<ToQspCliArguments>) =
        let sourcePath = results.GetResult ToQspCliArguments.Source_Path
        let source =
            match sourcePath with
            | "-" -> Clipboard.getText()
            | _ -> System.IO.File.ReadAllText sourcePath
        Document.parse source
        |> Result.map (fun markdownCyoa ->
            Qsp.toQsp markdownCyoa
            |> Ast.Document.print
                (IndentsOption.UsingSpaces 2)
                FormatConfig.Default
        )
        |> function
            | Ok str ->
                match results.TryGetResult ToQspCliArguments.Clipboard with
                | Some _ ->
                    Clipboard.setText str
                | None ->
                    printfn "%s" str
                0
            | Error errMsg ->
                eprintfn "%s" errMsg
                1

[<RequireQualifiedAccess>]
type CliArguments =
    | [<CliPrefix(CliPrefix.None)>] To_Mermaid of ParseResults<ToMermaidCliArguments>
    | [<CliPrefix(CliPrefix.None)>] To_Qsp of ParseResults<ToQspCliArguments>
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | To_Mermaid _ -> "convert to mermaid"
            | To_Qsp _ -> "convert to QSP"

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module CliArguments =
    let exec (results: ParseResults<CliArguments>) =
        match results.GetSubCommand() with
        | CliArguments.To_Mermaid results ->
            ToMermaidCliArguments.exec results
        | CliArguments.To_Qsp results ->
            ToQspCliArguments.exec results

[<EntryPoint>]
let main args =
    let argParser = ArgumentParser<CliArguments>(programName = "markdown-cyoa")
    match args with
    | [||] ->
        argParser.PrintUsage() |> printfn "%s"
        0
    | _ ->
        let results =
            try
                Ok (argParser.ParseCommandLine(args))
            with e ->
                Error e.Message

        match results with
        | Error errMsg ->
            printfn "%s" errMsg
            1

        | Ok results ->
            CliArguments.exec results
