using System.Xml.Linq;
using Emanate.Core.Configuration;
using NSubstitute;

namespace Emanate.UnitTests.Core.Configuration
{
    public class ConfigurationCaretakerBuilder
    {
        private readonly IDiskAccessor diskAccessor = Substitute.For<IDiskAccessor>();

        public ConfigurationCaretakerBuilder SetConfig(string xml)
        {
            if (!string.IsNullOrWhiteSpace(xml))
                diskAccessor.Load(Arg.Any<string>()).Returns(XDocument.Parse(xml));

            return this;
        }

        public ConfigurationCaretaker Build()
        {
            return new ConfigurationCaretaker(diskAccessor);
        }
    }
}