using Emanate.Core;
using Emanate.Model;
using NSubstitute;

namespace Emanate.UnitTests.Builders
{
    internal class InputDeviceBuilder : DeviceBuilder<SourceDevice>
    {
        public InputDeviceBuilder WithName(string name)
        {
            Device.Name.Returns(name);
            return this;
        }
    }
}