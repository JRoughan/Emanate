using Emanate.Core.Input;
using Emanate.Core.Output;
using Emanate.Service;
using Moq;
using NUnit.Framework;

namespace Emanate.UnitTests.Core
{
    [TestFixture]
    public class monitoring_service
    {
        [Test]
        public void should_populate_string_properties()
        {
            var monitor = new Mock<IBuildMonitor>();
            var output = new Mock<IOutput>();
            var service = new MonitoringService(monitor.Object, output.Object);
        }
    }
}
