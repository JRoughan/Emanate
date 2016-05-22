using Emanate.Core.Output;

namespace Emanate.Core.Input
{
    public interface IBuildMonitor
    {
        void BeginMonitoring();
        void EndMonitoring();
        void AddMapping(IDevice inputDevice, IOutputDevice outputDevice, InputInfo inputInfo);
    }
}