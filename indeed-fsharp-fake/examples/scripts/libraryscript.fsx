#r @"packages/FAKE/tools/FakeLib.dll"

open Fake

open System

module Libraryscript =

    //install the service using sc, see https://technet.microsoft.com/en-us/library/bb490995.aspx
    //https://stackoverflow.com/questions/27446800/fake-deploy-running-custom-windows-service-locks-deployment-files-and-thus-updat
    let InstallWindowsService host serviceName serviceExecutable =
        let result, messages = 
                ProcessHelper.ExecProcessRedirected
                    (fun info ->
                        info.FileName <- "sc"
                        //NOTE: binPath is absolute path to the executable
                        info.Arguments <- 
                            sprintf @"\\%s create ""%s"" binPath=""%s"" DisplayName=""%s"""
                                    host 
                                    serviceName 
                                    serviceExecutable
                                    serviceName
                        )
                    (TimeSpan.FromMinutes 1.0)
        for msg in messages do
            (if msg.IsError then traceError else traceImportant) msg.Message

        if not result then
            failwithf "Install of TestWindowsService returned with a non-zero exit code"                
        ()