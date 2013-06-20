using System;
using System.Collections.ObjectModel;
using Emanate.Core.Output;

namespace Emanate.Core.Configuration
{
    public interface IModuleConfiguration : IOriginator
    {
        string Key { get; }
        string Name { get; }

        Type GuiType { get; }

        ObservableCollection<IOutputProfile> Profiles { get; }
    }
}
