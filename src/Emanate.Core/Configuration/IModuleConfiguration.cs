using System;
using System.Collections.Generic;
using Emanate.Core.Output;

namespace Emanate.Core.Configuration
{
    public interface IModuleConfiguration : IOriginator
    {
        string Key { get; }
        string Name { get; }

        Type GuiType { get; }

        IEnumerable<IOutputProfile> Profiles { get; }
    }
}
