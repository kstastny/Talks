using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace TestWindowsService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                var parameter = string.Concat(args);

                switch (parameter)
                {
                    case "-install":
                        Console.WriteLine("Installing service...");
                        ManagedInstallerClass.InstallHelper(new[] {Assembly.GetExecutingAssembly().Location});
                        Console.WriteLine("Service successfully installed.");
                        break;

                    case "-uninstall":
                        Console.WriteLine("Uninstalling service...");
                        ManagedInstallerClass.InstallHelper(new[] {"/u", Assembly.GetExecutingAssembly().Location});
                        Console.WriteLine("Service successfully uninstalled.");
                        break;
                    default:
                        Console.WriteLine("Nothing happens");
                        break;
                }
            }
            else
            {
                var servicesToRun = new ServiceBase[]
                {
                    new TestService()
                };
                ServiceBase.Run(servicesToRun);
            }
        }
    }
}
