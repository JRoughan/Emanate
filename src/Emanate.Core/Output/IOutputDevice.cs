using System;
using Emanate.Core.Input;

namespace Emanate.Core.Output
{
    public interface IOutputDevice : IDevice
    {
        string Key { get; }
        string PhysicalDeviceId { get; }
        string Name { get; set; }

        string Type { get; }

        IProfile Profile { get; set; }

        bool IsAvailable { get; }
        void UpdateStatus(BuildState state, DateTimeOffset timeStamp);
    }
}
