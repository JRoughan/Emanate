using System;
using Emanate.Model;
using NSubstitute;

namespace Emanate.UnitTests.Builders
{
    internal class DeviceBuilder<TDevice>
        where TDevice : Entity
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

        public TDevice Build()
        {
            return Device;
        }
    }
}