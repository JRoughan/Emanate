using System.Collections.Generic;
using System.Threading;
using Emanate.Core.Input;
using Emanate.Core.Output;

namespace Emanate.Service
{
    public class EmanateConsole : IEmanateApp
    {
        private readonly IBuildMonitor monitor;
        private readonly IOutput output;
        private bool isRunning;
        private IEnumerable<InputInfo> inputs;

        public EmanateConsole(IBuildMonitor monitor, IOutput output)
        {
            this.output = output;
            this.monitor = monitor;
            this.monitor.StatusChanged += MonitorStatusChanged;
        }

        public void Start()
        {
            isRunning = true;
            monitor.BeginMonitoring(inputs);
            SpinWait.SpinUntil(() => !isRunning);
        }

        public void Stop()
        {
            monitor.EndMonitoring();

            isRunning = false;
        }

        private void MonitorStatusChanged(object sender, StatusChangedEventArgs e)
        {
            output.UpdateStatus(e.NewState, e.TimeStamp);
        }

        public void SetInputsToMonitor(IEnumerable<InputInfo> inputsToMonitor)
        {
            this.inputs = inputsToMonitor;
        }
    }
}