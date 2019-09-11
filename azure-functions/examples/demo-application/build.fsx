#r "paket: groupref Build //"

#load ".fake/build.fsx/intellisense.fsx"
open Fake
open Fake.Core
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators
open Fake.DotNet


let appSrc = "src"


Fake.Core.Target.create "Build" (fun _ ->
    DotNet.build (fun p -> { p with Configuration = DotNet.BuildConfiguration.Debug}) appSrc |> ignore
)



Fake.Core.Target.create "Run" (fun _ ->
    DotNet.exec (fun p -> {p with WorkingDirectory = appSrc}) "run" "" |> ignore
)

Fake.Core.Target.create "Publish" (fun _ ->
    DotNet.publish  (fun p -> { p with Configuration = DotNet.BuildConfiguration.Release ; OutputPath = Some "../../deploy" }) appSrc |> ignore
    File.delete "deploy/local.settings.json"
)

Fake.Core.Target.create "Clean" (fun _ -> 
    !! "src/*/bin"
    ++ "src/*/obj"
    ++ "tests/*/bin"
    ++ "tests/*/obj"
    ++ "deploy"
    |> Shell.deleteDirs
)


Fake.Core.Target.create "CleanBinObj" (fun _ -> 
    !! "src/*/bin"
    ++ "src/*/obj"
    ++ "tests/*/bin"
    ++ "tests/*/obj"
    |> Shell.deleteDirs
)


"Clean" ==> "Build" ==> "Publish"


// start build
Fake.Core.Target.runOrDefault "Build"