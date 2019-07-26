using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Emanate.Core;
using Emanate.Core.Configuration;
using NSubstitute;

namespace Emanate.UnitTests.Core.Configuration
{
    public class ConfigurationCaretakerBuilder
    {
        private readonly IDiskAccessor diskAccessor = Substitute.For<IDiskAccessor>();
        private readonly List<IModule> modules = new List<IModule>();

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

        public ConfigurationCaretaker Build()
        {
            return new ConfigurationCaretaker(diskAccessor, modules);
        }
    }
}