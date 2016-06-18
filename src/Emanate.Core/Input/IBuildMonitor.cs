using System.Collections.Generic;
using Emanate.Core.Output;

namespace Emanate.Core.Input
{
    public interface IBuildMonitorFactory
    {
        IBuildMonitor Create(IDevice device);
    }

    public interface IBuildMonitor
    {
        void BeginMonitoring();
        void EndMonitoring();
        void AddBuilds(IOutputDevice outputDevice, IEnumerable<string> inputs);
    }
}