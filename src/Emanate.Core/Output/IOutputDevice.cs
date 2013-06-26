using System.Collections.Generic;

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
    }
}
