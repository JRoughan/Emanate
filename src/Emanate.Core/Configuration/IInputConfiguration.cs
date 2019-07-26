using System.Collections.Generic;
using Emanate.Model;

namespace Emanate.Core.Configuration
{
    public interface IInputConfiguration : IConfiguration, IOriginator
    {
        string Key { get; }
        IEnumerable<SourceDevice> Devices { get; }
    }
}