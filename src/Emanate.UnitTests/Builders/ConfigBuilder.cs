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

        public ConfigBuilder WithMapping(DisplayConfiguration mapping, bool generateDependencies = true)
        {
            config.Mappings.Add(mapping);
            if (generateDependencies)
            {
                if (config.OutputDevices.All(d => d.Id != mapping.DisplayDeviceId))
                {
                    var outputDevice = new OutputDeviceBuilder()
                        .WithId(mapping.DisplayDeviceId)
                        .Build();
                    WithOutputDevice(outputDevice);
                }

                foreach (var inputGroup in mapping.SourceGroups)
                {
                    if (config.InputDevices.All(d => d.Id != inputGroup.SourceDeviceId))
                    {
                        var inputDevice = new InputDeviceBuilder()
                            .WithId(inputGroup.SourceDeviceId)
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
