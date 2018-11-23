using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace TestWindowsService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        private const string SERVICE_NAME = "ServiceName";

        public ProjectInstaller()
        {
            InitializeComponent();

            serviceInstaller.ServiceName = ConfigurationManager.AppSettings[SERVICE_NAME];
        }

        private void serviceProcessInstaller_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}