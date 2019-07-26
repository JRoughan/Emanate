using System;
using Emanate.Core.Input;
using Emanate.Model;

namespace Emanate.Core.Output
{
    public interface IOutputDevice : IDevice
    {
        string Type { get; }
        DisplayDeviceProfile Profile { get; set; }
        bool IsAvailable { get; }
        void UpdateStatus(BuildState state, DateTimeOffset timeStamp);
    }
}
