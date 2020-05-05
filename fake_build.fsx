open Fake.Core
open Fake.IO
open Fake.DotNet
open Fake.Core.TargetOperators
open Fake.IO.Globbing.Operators

let pclBuildDir = Path.getFullName "./FakeNukeTryout/bin/Debug"
let pclDir = Path.getFullName "./FakeNukeTryout" 
let androidBuildDir = Path.getFullName "./FakeNukeTryout.Android/bin/Debug"
let androidProj = Path.getFullName "./FakeNukeTryout.Android/FakeNukeTryout.Android.fsproj"
let apkOutputFile = Path.getFullName "./FakeNukeTryout.Android/bin/*/*.apk"
let artifactsFolder = Path.getFullName "./artifacts"

let runDotNet cmd dir =
    let result = DotNet.exec (DotNet.Options.withWorkingDirectory dir) cmd ""
    if result.ExitCode <> 0 then failwithf "'dotnet %s' failed in %s" cmd dir

Target.create "CleanPCL" (fun _ ->
    Shell.cleanDir pclBuildDir
    Trace.trace @"---PCL Project Cleaned---"
)

Target.create "RestorePackages" (fun _ ->
    NuGet.Restore.RestorePackages()
    Trace.trace @"---Packages Restored---"
)

Target.create "BuildPCL" (fun _ ->
    runDotNet "build" pclDir
    Trace.trace @"---PCL Project builded---"
)

Target.create "CleanAndroid" (fun _ ->
    Shell.cleanDir androidBuildDir
    Trace.trace @"---Android project cleaned---"
)

Target.create "BuildAndroid" (fun _ ->
    let setParameters (defaults: MSBuildParams) =
        { defaults with Verbosity = Some(Detailed)
                        Targets = ["Build"]
                        Properties = [ "Configuration", "Debug" ] }
    MSBuild.build setParameters androidProj
    Trace.trace @"---Android project builded---"
)

Target.create "PostBuildAndroid" (fun _ ->
    Directory.ensure artifactsFolder
    Trace.trace @"---Ensured artifacts folder is created---"
    let apkGlobbing = !! apkOutputFile
    for apkFile in apkGlobbing do
        Shell.moveFile artifactsFolder apkFile
        Trace.trace <| sprintf @"---Moved to Artifacts Folder %s---" apkFile
)

"CleanPCL" ==> "RestorePackages" ==> "BuildPCL" ==> "BuildAndroid" ==> "PostBuildAndroid"

Target.runOrDefault "PostBuildAndroid"