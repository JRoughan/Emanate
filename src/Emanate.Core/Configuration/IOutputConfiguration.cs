using System.Collections.Generic;
using Emanate.Core.Output;

namespace Emanate.Core.Configuration
{
    public interface IOutputConfiguration : IConfiguration, IOriginator
    {
        string Key { get; }

        IEnumerable<IProfile> Profiles { get; }

        IEnumerable<IOutputDevice> OutputDevices { get; }
    }
}
