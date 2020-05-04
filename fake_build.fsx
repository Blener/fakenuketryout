open Fake.Core
open Fake.IO
open Fake.DotNet
open Fake.Core.TargetOperators

let pclBuildDir = Path.getFullName "./FakeNukeTryout/bin/Debug"
let pclDir = Path.getFullName "./FakeNukeTryout" 
let androidBuildDir = Path.getFullName "./FakeNukeTryout.Android/bin/Debug"
let androidProj = Path.getFullName "./FakeNukeTryout.Android/FakeNukeTryout.Android.fsproj"

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
        { defaults with Verbosity = Some(Quiet)
                        Targets = ["Build"]
                        Properties = [ "Configuration", "Debug" ] }
    MSBuild.build setParameters androidProj
    Trace.trace @"---Android project builded---"
)

"CleanPCL" ==> "RestorePackages" ==> "BuildPCL" ==> "BuildAndroid"

Target.runOrDefault "BuildAndroid"