using System;
using System.Linq;
using System.Threading.Tasks;
using Emanate.Core;
using Emanate.Core.Input;

namespace Emanate.TeamCity2017
{
    public class TeamCity2017MonitorFactory : IBuildMonitorFactory
    {
        private readonly Func<IInputDevice, TeamCity2017Monitor> monitorFactory;

        public TeamCity2017MonitorFactory(Func<IInputDevice, TeamCity2017Monitor> monitorFactory)
        {
            this.monitorFactory = monitorFactory;
        }

        public IBuildMonitor Create(IInputDevice device)
        {
            return monitorFactory(device);
        }
    }
}