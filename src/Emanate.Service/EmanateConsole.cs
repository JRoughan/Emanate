using Emanate.Core.Input;
using Emanate.Core.Output;

namespace Emanate.Service
{
    public class EmanateConsole
    {
        private readonly IBuildMonitor monitor;
        private readonly IOutput output;

        public EmanateConsole(IBuildMonitor monitor, IOutput output)
        {
            this.output = output;
            this.monitor = monitor;
            this.monitor.StatusChanged += MonitorStatusChanged;
        }

        private void MonitorStatusChanged(object sender, StatusChangedEventArgs e)
        {
            output.UpdateStatus(e.NewState, e.TimeStamp);
        }

        public void Start()
        {
            monitor.BeginMonitoring();
        }

        public void Stop()
        {
            monitor.EndMonitoring();
        }
    }
}