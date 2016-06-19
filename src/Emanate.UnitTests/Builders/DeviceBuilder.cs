using System;
using Emanate.Core;
using NSubstitute;

namespace Emanate.UnitTests.Builders
{
    internal class DeviceBuilder<TDevice>
        where TDevice : class, IDevice
    {
        protected TDevice Device { get; }

        protected DeviceBuilder()
        {
            Device = Substitute.For<TDevice>();
        }

        public DeviceBuilder<TDevice> WithId(Guid id)
        {
            Device.Id.Returns(id);
            return this;
        }

        public DeviceBuilder<TDevice> WithName(string name)
        {
            Device.Name.Returns(name);
            return this;
        }

        public TDevice Build()
        {
            return Device;
        }
    }
}