using System.Collections.Generic;
using Emanate.Model;

namespace Emanate.Core.Configuration
{
    public interface IOutputConfiguration : IConfiguration, IOriginator
    {
        string Key { get; }

        IEnumerable<IProfile> Profiles { get; }

        IEnumerable<DisplayDevice> OutputDevices { get; }
    }
}
