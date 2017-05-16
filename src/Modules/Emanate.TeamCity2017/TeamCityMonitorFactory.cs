using System;
using System.Linq;
using System.Threading.Tasks;
using Emanate.Core;
using Emanate.Core.Input;

namespace Emanate.TeamCity2017
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