using System;
using Emanate.Core;
using Emanate.Core.Input;

namespace Emanate.TeamCity
{
    public class TeamCityMonitorFactory : IBuildMonitorFactory
    {
        private readonly Func<IInputDevice, TeamCityMonitor> monitorFactory;

        public TeamCityMonitorFactory(Func<IInputDevice, TeamCityMonitor> monitorFactory)
        {
            this.monitorFactory = monitorFactory;
        }

        public IBuildMonitor Create(IInputDevice device)
        {
            return monitorFactory(device);
        }
    }
}