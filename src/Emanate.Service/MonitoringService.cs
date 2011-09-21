using System;
using System.ServiceProcess;
using Emanate.Core;
using Emanate.Core.Input;
using Emanate.Core.Input.TeamCity;
using Emanate.Core.Output;

namespace Emanate.Service
{
    public partial class MonitoringService : ServiceBase
    {
        private readonly IBuildMonitor monitor;
        private readonly IOutput output;

        public MonitoringService()
        {
            InitializeComponent();
            var configuration = new ApplicationConfiguration();
            output = new DelcomOutput();
            monitor = new TeamCityMonitor(configuration);
            monitor.StatusChanged += MonitorStatusChanged;
        }

        public void Start()
        {
            OnStart(null);
        }

        private void MonitorStatusChanged(object sender, StatusChangedEventArgs e)
        {
            output.UpdateStatus(e.NewState);
        }

        protected override void OnStart(string[] args)
        {
            monitor.BeginMonitoring();
        }

        protected override void OnStop()
        {
            monitor.EndMonitoring();
        }
    }
}
