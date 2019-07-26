using System.Collections.Generic;
using System.Threading.Tasks;
using Emanate.Core.Output;

namespace Emanate.Core.Input
{
    public interface IBuildMonitorFactory<TDevice>
    {
        IBuildMonitor Create(TDevice device);
    }

    public interface IBuildMonitor
    {
        Task BeginMonitoring();
        void EndMonitoring();
        void AddBuilds(IOutputDevice outputDevice, IEnumerable<string> inputs);
    }
}