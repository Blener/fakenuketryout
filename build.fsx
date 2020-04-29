open Fake.Core

Target.create "Test" (fun _ ->
    printfn "Okay, it works."
)

Target.runOrDefault "Test"