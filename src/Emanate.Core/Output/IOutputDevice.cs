using System;
using System.Collections.Generic;
using Emanate.Core.Input;

namespace Emanate.Core.Output
{
    public interface IOutputDevice
    {
        string Key { get; }
        string Id { get; }
        string Name { get; }

        string Type { get; }

        List<InputInfo> Inputs { get; }
        IOutputProfile Profile { get; set; }

        bool IsAvailable { get; }
        void UpdateStatus(BuildState state, DateTimeOffset timeStamp);
    }
}
