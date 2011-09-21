using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Emanate.Core;
using Emanate.Core.Input;
using Emanate.Core.Input.TeamCity;

namespace Emanate.Service
{
    public partial class MonitoringService : ServiceBase
    {
        private readonly IBuildMonitor monitor;

        public MonitoringService()
        {
            InitializeComponent();
            var configuration = new ApplicationConfiguration();
            monitor = new TeamCityMonitor(configuration);
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
