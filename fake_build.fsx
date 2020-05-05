open Fake.Core
open Fake.IO
open Fake.DotNet
open Fake.Core.TargetOperators
open Fake.IO.Globbing.Operators
open Fake.DotNet.Xamarin
open System.IO

let pclBuildDir = "./FakeNukeTryout/bin/Release"
let androidBuildDir = "./FakeNukeTryout.Android/bin/Release"
let androidProj = "./FakeNukeTryout.Android/FakeNukeTryout.Android.fsproj"
let artifactsFolder = "./artifacts"

let runDotNet cmd dir =
    let result = DotNet.exec (DotNet.Options.withWorkingDirectory dir) cmd ""
    if result.ExitCode <> 0 then failwithf "'dotnet %s' failed in %s" cmd dir

Target.create "EnsureCleanFolders" (fun _ ->
    Directory.ensure pclBuildDir
    Shell.cleanDir pclBuildDir
    Trace.trace @"---PCL build folder Cleaned---"
    Directory.ensure androidBuildDir
    Shell.cleanDir androidBuildDir
    Trace.trace @"---Android build folder cleaned---"
    Directory.ensure artifactsFolder
    Shell.cleanDir artifactsFolder
    Trace.trace @"---Artifacts folder cleaned"
)

Target.create "BuildAndroid" (fun _ ->
    AndroidPackage (fun defaults ->
        { defaults with 
            ProjectPath = androidProj
            Configuration = "Release"
            OutputPath = androidBuildDir })
    |> fun apkFile -> apkFile.CopyTo(Path.Combine(artifactsFolder, apkFile.Name)) |> ignore
    Trace.trace @"---Android project builded---"
)

"EnsureCleanFolders" ==> "BuildAndroid"

Target.runOrDefault "BuildAndroid"