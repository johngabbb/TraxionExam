using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace Exam2
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        private ServiceProcessInstaller processInstaller;
        private ServiceInstaller serviceInstaller;

        public ProjectInstaller()
        {
            InitializeComponent();

            // Instantiate installers for process and service
            processInstaller = new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalService // Use Local Service account
            };

            serviceInstaller = new ServiceInstaller
            {
                ServiceName = "FileWatcherService", 
                DisplayName = "File Watcher Service", 
                Description = "A service that monitors folder1 and moves files to folder2.", 
                StartType = ServiceStartMode.Automatic 
            };

            // Add installers to the installer collection
            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
