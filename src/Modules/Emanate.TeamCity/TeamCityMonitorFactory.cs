using System;
using Emanate.Core.Input;

namespace Emanate.TeamCity
{
    public class TeamCityMonitorFactory : IBuildMonitorFactory<TeamCityDevice>
    {
        private readonly Func<TeamCityDevice, TeamCityMonitor> monitorFactory;

        public TeamCityMonitorFactory(Func<TeamCityDevice, TeamCityMonitor> monitorFactory)
        {
            this.monitorFactory = monitorFactory;
        }

        public IBuildMonitor Create(TeamCityDevice device)
        {
            return monitorFactory(device);
        }
    }
}