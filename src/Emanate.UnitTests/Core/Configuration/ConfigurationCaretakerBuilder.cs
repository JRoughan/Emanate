using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Autofac.Features.Indexed;
using Emanate.Core;
using Emanate.Core.Configuration;
using NSubstitute;

namespace Emanate.UnitTests.Core.Configuration
{
    public class ConfigurationCaretakerBuilder
    {
        private readonly IDiskAccessor diskAccessor = Substitute.For<IDiskAccessor>();
        private readonly List<IModule> modules = new List<IModule>();
        private readonly IIndex<string, IOutputConfiguration> outputConfigurations = Substitute.For<IIndex<string, IOutputConfiguration>>();
        private readonly IIndex<string, IInputConfiguration> inputConfigurations = Substitute.For<IIndex<string, IInputConfiguration>>();
        private readonly List<Lazy<IOutputConfiguration>> lazyOutputConfigurations = new List<Lazy<IOutputConfiguration>>();
        private readonly List<Lazy<IInputConfiguration>> lazyInputConfigurations = new List<Lazy<IInputConfiguration>>();

        public ConfigurationCaretakerBuilder SetConfig(string xml)
        {
            if (!string.IsNullOrWhiteSpace(xml))
                diskAccessor.Load(Arg.Any<string>()).Returns(XDocument.Parse(xml));

            return this;
        }

        public ConfigurationCaretakerBuilder AddModule(IModule module)
        {
            modules.Add(module);
            return this;
        }

        public ConfigurationCaretakerBuilder AddModule(string key, Direction direction)
        {
            var module = Substitute.For<IModule>();
            module.Key.Returns(key);
            module.Direction.Returns(direction);
            AddModule(module);
            return this;
        }

        public ConfigurationCaretakerBuilder AddInputConfiguration(string key, IInputConfiguration inputConfiguration)
        {
            inputConfigurations[key].Returns(inputConfiguration);
            lazyInputConfigurations.Add(new Lazy<IInputConfiguration>(() => inputConfiguration));
            return this;
        }

        public ConfigurationCaretakerBuilder AddOutputConfiguration(string key, IOutputConfiguration outputConfiguration)
        {
            outputConfigurations[key].Returns(outputConfiguration);
            lazyOutputConfigurations.Add(new Lazy<IOutputConfiguration>(() => outputConfiguration));
            return this;
        }

        public ConfigurationCaretaker Build()
        {
            return new ConfigurationCaretaker(diskAccessor, modules, outputConfigurations, inputConfigurations, lazyOutputConfigurations, lazyInputConfigurations);
        }
    }
}