using System;
using Emanate.Core;
using Emanate.Core.Input;

namespace Emanate.Vso
{
    public class VsoMonitorFactory : IBuildMonitorFactory
    {
        private readonly Func<IDevice, VsoMonitor> monitorFactory;

        public VsoMonitorFactory(Func<IDevice, VsoMonitor> monitorFactory)
        {
            this.monitorFactory = monitorFactory;
        }

        public IBuildMonitor Create(IDevice device)
        {
            return monitorFactory(device);
        }
    }
}