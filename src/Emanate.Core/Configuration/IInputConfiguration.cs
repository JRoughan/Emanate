using System.Collections.Generic;

namespace Emanate.Core.Configuration
{
    public interface IInputConfiguration : IConfiguration, IOriginator
    {
        string Key { get; }
        IEnumerable<IInputDevice> Devices { get; }
    }
}