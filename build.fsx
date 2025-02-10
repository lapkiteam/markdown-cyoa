// --------------------------------------------------------------------------------------
// FAKE build script
// --------------------------------------------------------------------------------------
#r "nuget: Fake.Core.Target, 6.0"
#r "nuget: Fake.DotNet.Cli, 6.0"
#r "nuget: Fake.IO.FileSystem, 6.0"
#r "nuget: Fake.Core.ReleaseNotes, 6.0"

open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators

// Boilerplate
System.Environment.GetCommandLineArgs()
|> Array.skip 2 // skip fsi.exe; build.fsx
|> Array.toList
|> Fake.Core.Context.FakeExecutionContext.Create false __SOURCE_FILE__
|> Fake.Core.Context.RuntimeContext.Fake
|> Fake.Core.Context.setExecutionContext

Target.initEnvironment ()
// --------------------------------------------------------------------------------------
// Build variables
// --------------------------------------------------------------------------------------
let deployDir = Path.getFullName "./deploy"

let coreProjectPath = "src/Core/Core.fsproj"
let coreProjectDirectory = Path.getDirectory coreProjectPath
let coreDeployDirectory = deployDir </> "Core"
let coreReleasePath = coreProjectDirectory </> "RELEASE_NOTES.md"

let cliProjectPath = "src/Cli/Cli.fsproj"
let cliProjectDirectory = Path.getDirectory cliProjectPath
let cliDeployDirectory = sprintf "%s/Cli" deployDir
let releasePath = "RELEASE_NOTES.md"

let testProjPath = "tests/Tests.fsproj"
let testsProjDir = Path.getDirectory testProjPath

let commonBuildArgs = "-c Release"
// --------------------------------------------------------------------------------------
// Helpers
// --------------------------------------------------------------------------------------
open Fake.DotNet

let dotnet cmd workingDir =
    let result = DotNet.exec (DotNet.Options.withWorkingDirectory workingDir) cmd ""
    if result.ExitCode <> 0 then failwithf "'dotnet %s' failed in %s" cmd workingDir

module XmlText =
    let escape rawText =
        let doc = new System.Xml.XmlDocument()
        let node = doc.CreateElement("root")
        node.InnerText <- rawText
        node.InnerXml

let cleanBinAndObj projectPath =
    try
        Shell.cleanDirs [
            projectPath </> "bin"
            projectPath </> "obj"
        ]
    with _ -> ()
// --------------------------------------------------------------------------------------
// Targets
// --------------------------------------------------------------------------------------
Target.create "Clean" (fun _ ->
    cleanBinAndObj testsProjDir
    Shell.cleanDir deployDir
)

Target.create "CoreClean" (fun _ ->
    cleanBinAndObj coreProjectDirectory
    Shell.cleanDir coreDeployDirectory
)

Target.create "CoreMeta" (fun _ ->
    let release = ReleaseNotes.load coreReleasePath

    [
        "<Project xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">"
        "<ItemGroup>"
        "    <PackageReference Include=\"Microsoft.SourceLink.GitHub\" Version=\"1.0.0\" PrivateAssets=\"All\"/>"
        "</ItemGroup>"
        "<PropertyGroup>"
        "    <EmbedUntrackedSources>true</EmbedUntrackedSources>"
        "    <PackageProjectUrl>https://github.com/lapkiteam/markdown-cyoa/tree/main/src/Core</PackageProjectUrl>"
        "    <PackageLicenseExpression>MIT</PackageLicenseExpression>"
        "    <RepositoryUrl>https://github.com/lapkiteam/markdown-cyoa.git</RepositoryUrl>"
        sprintf "    <PackageReleaseNotes>%s</PackageReleaseNotes>"
            (String.concat "\n" release.Notes |> XmlText.escape)
        "    <PackageTags>cyoa;fsharp;markdown;library</PackageTags>"
        "    <Authors>gretmn102</Authors>"
        sprintf "    <Version>%s</Version>" (string release.SemVer)
        "</PropertyGroup>"
        "</Project>"
    ]
    |> File.write false (
        coreProjectDirectory </> "Directory.Build.props"
    )
)

Target.create "CoreBuild" (fun _ ->
    coreProjectDirectory
    |> dotnet (sprintf "build %s" commonBuildArgs)
)

Target.create "CoreDeploy" (fun _ ->
    coreProjectDirectory
    |> dotnet (sprintf "build %s -o \"%s\"" commonBuildArgs coreDeployDirectory)
)

Target.create "CorePack" (fun _ ->
    coreProjectDirectory
    |> dotnet (sprintf "pack %s -o \"%s\"" commonBuildArgs coreDeployDirectory)
)

Target.create "CorePushToGitlab" (fun _ ->
    let packPathPattern = sprintf "%s/*.nupkg" coreDeployDirectory
    let packPath =
        !! packPathPattern |> Seq.tryExactlyOne
        |> Option.defaultWith (fun () -> failwithf "'%s' not found" packPathPattern)

    deployDir
    |> dotnet (sprintf "nuget push -s %s %s" "gitlab" packPath)
)

Target.create "CliClean" (fun _ ->
    cleanBinAndObj cliProjectDirectory
    Shell.cleanDir cliDeployDirectory
)


Target.create "CliMeta" (fun _ ->
    let release = ReleaseNotes.load releasePath

    [
        "<Project xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">"
        "<ItemGroup>"
        "    <PackageReference Include=\"Microsoft.SourceLink.GitHub\" Version=\"1.0.0\" PrivateAssets=\"All\"/>"
        "</ItemGroup>"
        "<PropertyGroup>"
        "    <EmbedUntrackedSources>true</EmbedUntrackedSources>"
        "    <PackageProjectUrl>https://github.com/lapkiteam/markdown-cyoa/tree/main/src/Cli</PackageProjectUrl>"
        "    <PackageLicenseExpression>MIT</PackageLicenseExpression>"
        "    <RepositoryUrl>https://github.com/lapkiteam/markdown-cyoa.git</RepositoryUrl>"
        sprintf "    <PackageReleaseNotes>%s</PackageReleaseNotes>"
            (String.concat "\n" release.Notes |> XmlText.escape)
        "    <PackageTags>cyoa;fsharp;markdown;cli</PackageTags>"
        "    <Authors>gretmn102</Authors>"
        sprintf "    <Version>%s</Version>" (string release.SemVer)
        "</PropertyGroup>"
        "</Project>"
    ]
    |> File.write false (
        cliProjectDirectory </> "Directory.Build.props"
    )
)

Target.create "CliBuild" (fun _ ->
    cliProjectDirectory
    |> dotnet (sprintf "build %s" commonBuildArgs)
)

Target.create "CliDeploy" (fun _ ->
    cliProjectDirectory
    |> dotnet (sprintf "build %s -o \"%s\"" commonBuildArgs cliDeployDirectory)
)

Target.create "CliPack" (fun _ ->
    cliProjectDirectory
    |> dotnet (sprintf "pack %s -o \"%s\"" commonBuildArgs cliDeployDirectory)
)

Target.create "CliPushToGitlab" (fun _ ->
    let packPathPattern = sprintf "%s/*.nupkg" cliDeployDirectory
    let packPath =
        !! packPathPattern |> Seq.tryExactlyOne
        |> Option.defaultWith (fun () -> failwithf "'%s' not found" packPathPattern)

    deployDir
    |> dotnet (sprintf "nuget push -s %s %s" "gitlab" packPath)
)

Target.create "TestsRun" (fun _ ->
    testsProjDir
    |> dotnet (sprintf "run %s" commonBuildArgs)
)

// --------------------------------------------------------------------------------------
// Build order
// --------------------------------------------------------------------------------------
open Fake.Core.TargetOperators

"Build"

"CliClean"
  ==> "CliDeploy"

"CliClean"
  ==> "CliMeta"
  ==> "CliPack"
  ==> "CliPushToGitlab"

"CoreClean"
  ==> "CoreMeta"
  ==> "CorePack"
  ==> "CorePushToGitlab"

"TestsRun"

Target.runOrDefault "CliDeploy"
