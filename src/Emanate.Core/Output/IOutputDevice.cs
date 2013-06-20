using System;
using System.Collections.Generic;
using Emanate.Core.Configuration;

namespace Emanate.Core.Output
{
    public interface IOutputDevice : IOriginator
    {
        string Key { get; }
        string Name { get; }

        string Type { get; }

        List<InputInfo> Inputs { get; }
        string Profile { get; set; }
    }
}
