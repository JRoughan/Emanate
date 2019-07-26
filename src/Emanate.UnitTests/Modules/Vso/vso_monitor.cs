using System;
using System.Threading.Tasks;
using Emanate.Core;
using Emanate.Core.Output;
using Emanate.Vso;
using NSubstitute;
using Xunit;

namespace Emanate.UnitTests.Modules.Vso
{
    public class vso_monitor
    {
        private static readonly Guid buildId1 = Guid.NewGuid();
        private static readonly Guid buildId2 = Guid.NewGuid();
        private static readonly Guid buildId3 = Guid.NewGuid();

        [Fact]
        public async void should_retrieve_builds_if_output_device_available()
        {
            var inputDevice = Substitute.For<VsoDevice>();
            var connection = Substitute.For<IVsoConnection>();
            var outputDevice = Substitute.For<IOutputDevice>();
            outputDevice.IsAvailable.Returns(true);
            var inputs = new[] { $"{buildId1}:1" };
            var monitor = new VsoMonitor(inputDevice, d => connection);

            monitor.AddBuilds(outputDevice, inputs);

            await monitor.BeginMonitoring();

            await connection.Received().GetBuild(buildId1, 1);
        }

        [Fact]
        public async void should_not_retrieve_builds_if_output_device_unavailable()
        {
            var inputDevice = Substitute.For<VsoDevice>();
            var connection = Substitute.For<IVsoConnection>();
            var outputDevice = Substitute.For<IOutputDevice>();
            outputDevice.IsAvailable.Returns(false);
            var inputs = new[] { $"{buildId1}:1" };
            var monitor = new VsoMonitor(inputDevice, d => connection);

            monitor.AddBuilds(outputDevice, inputs);

            await monitor.BeginMonitoring();

            await connection.DidNotReceiveWithAnyArgs().GetBuild(buildId1, 1);
        }

        [Fact]
        public async void should_retrieve_builds_for_all_available_devices()
        {
            var inputDevice = Substitute.For<VsoDevice>();
            var connection = Substitute.For<IVsoConnection>();

            var outputDevice1 = Substitute.For<IOutputDevice>();
            outputDevice1.IsAvailable.Returns(true);
            var inputs1 = new[] { $"{buildId1}:1", $"{buildId2}:2" };

            var outputDevice2 = Substitute.For<IOutputDevice>();
            outputDevice2.IsAvailable.Returns(false);
            var inputs2 = new[] { $"{buildId3}:3" };

            var monitor = new VsoMonitor(inputDevice, d => connection);
            monitor.AddBuilds(outputDevice1, inputs1);
            monitor.AddBuilds(outputDevice2, inputs2);

            await monitor.BeginMonitoring();

            await Task.WhenAll(
                connection.Received().GetBuild(buildId1, 1),
                connection.Received().GetBuild(buildId2, 2),
                connection.DidNotReceive().GetBuild(buildId3, 3));
        }
    }
}
