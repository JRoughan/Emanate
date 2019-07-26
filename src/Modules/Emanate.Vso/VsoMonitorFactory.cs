using System;
using Emanate.Core.Input;

namespace Emanate.Vso
{
    public class VsoMonitorFactory : IBuildMonitorFactory<VsoDevice>
    {
        private readonly Func<VsoDevice, VsoMonitor> monitorFactory;

        public VsoMonitorFactory(Func<VsoDevice, VsoMonitor> monitorFactory)
        {
            this.monitorFactory = monitorFactory;
        }

        public IBuildMonitor Create(VsoDevice device)
        {
            return monitorFactory(device);
        }
    }
}