using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNFakeLibrary
{
    public class WindowsServiceHelper
    {
        public void InstallWindowsService(string hostName, string serviceName, string serviceExecutable)
        {
            var processStartInfo = new ProcessStartInfo("sc",
                $"\\\\{hostName} create \"{serviceName}\" binPath=\"{serviceExecutable}\" DisplayName=\"{serviceName}\"");
            processStartInfo.RedirectStandardError = true;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.UseShellExecute = false;

            var process = Process.Start(processStartInfo);

            if (process != null)
            {
                if (!process.WaitForExit(60 * 1000))
                    process.Kill();

                Console.WriteLine(process.StandardOutput.ReadToEnd());
                if (process.ExitCode > 0)
                    throw new Exception("Error installing service: " + process.ExitCode);
            }
        }
    }
}