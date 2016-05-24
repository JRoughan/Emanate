using System;
using Emanate.Core.Input;

namespace Emanate.Core.Output
{
    public interface IOutputDevice : IDevice
    {
        string Type { get; }
        IProfile Profile { get; set; }
        bool IsAvailable { get; }
        void UpdateStatus(BuildState state, DateTimeOffset timeStamp);
    }
}
