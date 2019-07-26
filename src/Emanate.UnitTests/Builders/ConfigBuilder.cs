using System.Linq;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Output;
using Emanate.Model;

namespace Emanate.UnitTests.Builders
{
    internal class ConfigBuilder
    {
        private readonly GlobalConfig config;

        public ConfigBuilder()
        {
            config = new GlobalConfig();
        }

        public ConfigBuilder WithInputDevice(SourceDevice device)
        {
            config.InputDevices.Add(device);
            return this;
        }

        public ConfigBuilder WithOutputDevice(DisplayDevice device)
        {
            config.OutputDevices.Add(device);
            return this;
        }

        public ConfigBuilder WithMapping(Mapping mapping, bool generateDependencies = true)
        {
            config.Mappings.Add(mapping);
            if (generateDependencies)
            {
                if (config.OutputDevices.All(d => d.Id != mapping.OutputDeviceId))
                {
                    var outputDevice = new OutputDeviceBuilder()
                        .WithId(mapping.OutputDeviceId)
                        .Build();
                    WithOutputDevice(outputDevice);
                }

                foreach (var inputGroup in mapping.InputGroups)
                {
                    if (config.InputDevices.All(d => d.Id != inputGroup.InputDeviceId))
                    {
                        var inputDevice = new InputDeviceBuilder()
                            .WithId(inputGroup.InputDeviceId)
                            .Build();
                        WithInputDevice(inputDevice);
                    }
                }
            }
            return this;
        }

        public GlobalConfig Build()
        {
            return config;
        }
    }
}
