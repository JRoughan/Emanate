using System;
using System.Collections.Generic;
using Emanate.Core.Input;

namespace Emanate.Core.Output
{
    public interface IOutputDevice : IDevice
    {
        string Key { get; }
        string Id { get; }
        string Name { get; set; }

        string Type { get; }

        List<InputInfo> Inputs { get; }
        IProfile Profile { get; set; }

        bool IsAvailable { get; }
        void UpdateStatus(BuildState state, DateTimeOffset timeStamp);
    }
}
