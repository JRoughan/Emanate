using System;
using Emanate.Core;
using Emanate.Core.Input;

namespace Emanate.TeamCity
{
    public class TeamCityMonitorFactory : IBuildMonitorFactory
    {
        private readonly Func<IDevice, TeamCityMonitor> monitorFactory;

        public TeamCityMonitorFactory(Func<IDevice, TeamCityMonitor> monitorFactory)
        {
            this.monitorFactory = monitorFactory;
        }

        public IBuildMonitor Create(IDevice device)
        {
            return monitorFactory(device);
        }
    }
}