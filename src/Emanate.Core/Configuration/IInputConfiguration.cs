﻿using System.Collections.Generic;

namespace Emanate.Core.Configuration
{
    public interface IInputConfiguration : IConfiguration, IOriginator
    {
        string Key { get; }
        string Name { get; }

        IEnumerable<IDevice> Devices { get; }
    }
}