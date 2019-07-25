using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Emanate.Core.Input;
using Microsoft.Extensions.Hosting;

namespace Emanate.Web
{
    public class SourceMonitoringService : IHostedService
    {
        private readonly IEnumerable<IBuildMonitor> sourceMonitors;

        public SourceMonitoringService(IEnumerable<IBuildMonitor> sourceMonitors)
        {
            this.sourceMonitors = sourceMonitors;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var sourceMonitor in sourceMonitors)
            {
                await sourceMonitor.BeginMonitoring();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var sourceMonitor in sourceMonitors)
            {
                sourceMonitor.EndMonitoring();
            }

            return Task.CompletedTask;
        }
    }
}
