module MarkdownCyoa.Cli
open MarkdownCyoa.Core
open MarkdownCyoa.Core.Parser
open FMermaid.Api
open Argu
open FsharpMyExtension.ShowList

[<RequireQualifiedAccess>]
type ToMermaidCliArguments =
    | [<MainCommand; ExactlyOnce; Last>] Source_Path of path: string
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Source_Path _ -> "path to a markdown cyoa document"

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module ToMermaidCliArguments =
    let exec (results: ParseResults<ToMermaidCliArguments>) =
        let sourcePath = results.GetResult ToMermaidCliArguments.Source_Path
        let source = System.IO.File.ReadAllText sourcePath
        Document.parse source
        |> Result.map (fun markdownCyoa ->
            Mermaid.toMermaid markdownCyoa
            |> Flowchart.Document.Show.shows (
                showString "  "
            )
            |> joinEmpty "\n"
            |> show
        )

[<RequireQualifiedAccess>]
type ToQspCliArguments =
    | [<MainCommand; ExactlyOnce; Last>] Source_Path of path: string
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Source_Path _ -> "path to a markdown cyoa document"

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module ToQspCliArguments =
    open Qsp.Printer

    let exec (results: ParseResults<ToQspCliArguments>) =
        let sourcePath = results.GetResult ToQspCliArguments.Source_Path
        let source = System.IO.File.ReadAllText sourcePath
        Document.parse source
        |> Result.map (fun markdownCyoa ->
            Qsp.toQsp markdownCyoa
            |> Ast.Document.print
                (IndentsOption.UsingSpaces 2)
                FormatConfig.Default
        )

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
        let result =
            match results.GetSubCommand() with
            | CliArguments.To_Mermaid results ->
                ToMermaidCliArguments.exec results
            | CliArguments.To_Qsp results ->
                ToQspCliArguments.exec results
        match result with
        | Ok str ->
            printfn "%s" str
            0
        | Error errMsg ->
            eprintfn "%s" errMsg
            1

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
