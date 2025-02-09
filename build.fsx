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
let mainProjPath = "src/Cli.fsproj"
let mainProjDir = Path.getDirectory mainProjPath

let testProjPath = "tests/Tests.fsproj"
let testsProjDir = Path.getDirectory testProjPath

let deployDir = Path.getFullName "./deploy"

let releasePath = "RELEASE_NOTES.md"
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
// --------------------------------------------------------------------------------------
// Targets
// --------------------------------------------------------------------------------------
Target.create "Clean" (fun _ ->
    let cleanBinAndObj projectPath =
        try
            Shell.cleanDirs [
                projectPath </> "bin"
                projectPath </> "obj"
            ]
        with _ -> ()
    cleanBinAndObj mainProjDir
    cleanBinAndObj testsProjDir
    Shell.cleanDir deployDir
)

Target.create "Meta" (fun _ ->
    let release = ReleaseNotes.load releasePath

    [
        "<Project xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">"
        "<ItemGroup>"
        "    <PackageReference Include=\"Microsoft.SourceLink.GitHub\" Version=\"1.0.0\" PrivateAssets=\"All\"/>"
        "</ItemGroup>"
        "<PropertyGroup>"
        "    <EmbedUntrackedSources>true</EmbedUntrackedSources>"
        "    <PackageProjectUrl>https://github.com/gretmn102/markdown-cyoa</PackageProjectUrl>"
        "    <PackageLicenseExpression>MIT</PackageLicenseExpression>"
        "    <RepositoryUrl>https://github.com/gretmn102/markdown-cyoa.git</RepositoryUrl>"
        sprintf "    <PackageReleaseNotes>%s</PackageReleaseNotes>"
            (String.concat "\n" release.Notes |> XmlText.escape)
        "    <PackageTags>fsharp;markdown</PackageTags>"
        "    <Authors>gretmn102</Authors>"
        sprintf "    <Version>%s</Version>" (string release.SemVer)
        "</PropertyGroup>"
        "</Project>"
    ]
    |> File.write false "Directory.Build.props"
)

let commonBuildArgs = "-c Release"

Target.create "Build" (fun _ ->
    mainProjDir
    |> dotnet (sprintf "build %s" commonBuildArgs)
)

Target.create "Deploy" (fun _ ->
    mainProjDir
    |> dotnet (sprintf "build %s -o \"%s\"" commonBuildArgs deployDir)
)

Target.create "Pack" (fun _ ->
    mainProjDir
    |> dotnet (sprintf "pack %s -o \"%s\"" commonBuildArgs deployDir)
)

Target.create "PushToGitlab" (fun _ ->
    let packPathPattern = sprintf "%s/*.nupkg" deployDir
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

"Clean"
  ==> "Deploy"

"Clean"
  ==> "Meta"
  ==> "Pack"
  ==> "PushToGitlab"

"TestsRun"

Target.runOrDefault "Deploy"
