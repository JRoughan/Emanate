using System.Collections.Generic;
using System.ServiceProcess;
using Emanate.Core.Input;
using Emanate.Core.Output;

namespace Emanate.Service
{
    public partial class EmanateService : ServiceBase
    {
        private readonly IBuildMonitor monitor;
        private readonly IOutput output;
        private IEnumerable<InputInfo> inputs;

        public EmanateService(IBuildMonitor monitor, IOutput output)
        {
            InitializeComponent();

            this.output = output;
            this.monitor = monitor;
            this.monitor.StatusChanged += MonitorStatusChanged;
        }

        public void SetInputsToMonitor(IEnumerable<InputInfo> inputsToMonitor)
        {
            this.inputs = inputsToMonitor;
        }

        private void MonitorStatusChanged(object sender, StatusChangedEventArgs e)
        {
            output.UpdateStatus(e.NewState, e.TimeStamp);
        }

        protected override void OnStart(string[] args)
        {
            monitor.BeginMonitoring(inputs);
        }

        protected override void OnStop()
        {
            monitor.EndMonitoring();
        }
    }
}
