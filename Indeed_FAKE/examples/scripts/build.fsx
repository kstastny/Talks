// --------------------------------------------------------------------------------------
// FAKE build script
// --------------------------------------------------------------------------------------

#r @"packages/FAKE/tools/FakeLib.dll"

//load external script
#load "libraryscript.fsx"
//and open the module in external script
open Libraryscript

//reference library
 #r @"../src/CNFakeLibrary/bin/Debug/CNFakeLibrary.dll"
open CNFakeLibrary

open Fake

open System
open System.IO



let sln = @"..\src\TestApplication.sln"

let outputPath = getBuildParamOrDefault "output" @"..\build"

let deployPath = getBuildParamOrDefault "deployPath" @"..\testdeploy"


// Copies binaries from default VS location to expected bin folder
// But keeps a subdirectory structure for each project in the
// src folder to support multiple project outputs
Target "CopyBinaries" (fun _ ->
    let slnPath = Path.GetDirectoryName(sln)

    !! (slnPath @@ "**/*.??proj")
    |>  Seq.map (fun f -> ((System.IO.Path.GetDirectoryName f) @@ "bin/Debug", outputPath @@ (System.IO.Path.GetFileNameWithoutExtension f)))
    |>  Seq.iter (fun (fromDir, toDir) -> CopyDir toDir fromDir (fun _ -> true))
)

// --------------------------------------------------------------------------------------
// Clean build results

Target "Clean" (fun _ ->
    CleanDirs [outputPath]
)

// --------------------------------------------------------------------------------------
// Build library & test project

Target "BuildSolution" (fun _ ->
    tracefn "Building %s" sln
    
    !! sln
    |> MSBuildDebug "" "Rebuild"
    |> ignore
)

//no op target to specify sequence of targets correctly
Target "Build" DoNothing



let testWinserviceHost = "localhost"
let testWinserviceName = "FAKE Service"

Target "CopyWindowsService" (fun _ ->
    
    let fullDeployPath = Path.GetFullPath(deployPath)
    tracefn "Copying service to %s" fullDeployPath

    // let fileIncludes = (!! "**/*"
    //                         -- "**/*.pdb")
    //                         .SetBaseDirectory(outputPath @@ "TestWindowsService")

    // [fileIncludes]
    //     |> FileHelper.CopyWithSubfoldersTo fullDeployPath

    // more CSharpy programming example
    let filesToCopy = (Include "**/*").ButNot("**/*.pdb")
    let filesToCopy = filesToCopy.SetBaseDirectory(System.IO.Path.Combine(outputPath, "TestWindowsService"))
    FileHelper.CopyWithSubfoldersTo fullDeployPath [filesToCopy]
    )

Target "DeployWindowsService" (fun _ ->
    tracefn "Deploying windows service"
    let fullDeployPath = Path.GetFullPath(deployPath)

    //modify service filename - use XmlPoke https://fake.build/apidocs/fake-xmlhelper.html
    XMLHelper.XmlPoke
         (fullDeployPath @@ "TestWindowsService.exe.config")
         "/configuration/appSettings/add[@key = 'ServiceName']/@value" 
         testWinserviceName
    
    //install the windows service using SC
    let serviceExists =
        ServiceControllerHelpers.checkRemoteServiceExists testWinserviceHost testWinserviceName

    if not serviceExists then
        //Libraryscript.InstallWindowsService testWinserviceHost testWinserviceName (fullDeployPath @@ "TestWindowsService.exe")
        let serviceHelper = CNFakeLibrary.WindowsServiceHelper()
        serviceHelper.InstallWindowsService(testWinserviceHost, testWinserviceName, (fullDeployPath @@ "TestWindowsService.exe"))
    else
        ()

    //start the windows service
    ProcessHelper.StartRemoteService testWinserviceHost testWinserviceName
)


"Clean"
    ==> "BuildSolution"
    ==> "CopyBinaries"
    ==> "Build"


"Build"
    ==> "CopyWindowsService"
    ==> "DeployWindowsService"



RunTargetOrDefault "Build"
