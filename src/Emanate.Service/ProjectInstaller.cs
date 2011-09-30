using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;


namespace Emanate.Service
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            AfterInstall += StartService;
            InitializeComponent();
        }

        void StartService(object sender, InstallEventArgs e)
        {
            try
            {
                var serviceController = new ServiceController("EmanateService");
                serviceController.Start();
            }
            // Don't care if it can't be started automatically; The installation was still successful
            catch (Exception)
            {
                // TODO: Log to event log?
            }
        }
    }
}
