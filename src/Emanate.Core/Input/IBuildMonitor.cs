using System.Collections.Generic;
using Emanate.Core.Output;

namespace Emanate.Core.Input
{
    public interface IBuildMonitor
    {
        void BeginMonitoring();
        void EndMonitoring();
        void SetDevice(IDevice device);
        void AddBuilds(IOutputDevice outputDevice, IEnumerable<InputInfo> inputs);
    }
}