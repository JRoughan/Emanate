using System.ServiceProcess;
using Emanate.Core.Input;
using Emanate.Core.Output;

namespace Emanate.Service
{
    public partial class MonitoringService : ServiceBase
    {
        private readonly IBuildMonitor monitor;
        private readonly IOutput output;

        public MonitoringService(IBuildMonitor monitor, IOutput output)
        {
            InitializeComponent();

            this.output = output;
            this.monitor = monitor;
            this.monitor.StatusChanged += MonitorStatusChanged;
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
