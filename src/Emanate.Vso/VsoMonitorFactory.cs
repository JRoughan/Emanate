using System;
using Emanate.Core;
using Emanate.Core.Input;

namespace Emanate.Vso
{
    public class VsoMonitorFactory : IBuildMonitorFactory
    {
        private readonly Func<IInputDevice, VsoMonitor> monitorFactory;

        public VsoMonitorFactory(Func<IInputDevice, VsoMonitor> monitorFactory)
        {
            this.monitorFactory = monitorFactory;
        }

        public IBuildMonitor Create(IInputDevice device)
        {
            return monitorFactory(device);
        }
    }
}