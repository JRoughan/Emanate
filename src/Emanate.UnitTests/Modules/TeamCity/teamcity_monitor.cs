using System.Threading.Tasks;
using Emanate.Core;
using Emanate.Core.Output;
using Emanate.TeamCity;
using NSubstitute;
using Xunit;

namespace Emanate.UnitTests.Modules.TeamCity
{
    public class teamcity_monitor
    {
        private static readonly string buildId1 = "buildId1";
        private static readonly string buildId2 = "buildId2";
        private static readonly string buildId3 = "buildId3";

        [Fact]
        public async void should_retrieve_builds_if_output_device_available()
        {
            var inputDevice = Substitute.For<IInputDevice>();
            var connection = Substitute.For<ITeamCityConnection>();
            connection.GetBuild(Arg.Any<string>()).ReturnsForAnyArgs("<builds />");
            var outputDevice = Substitute.For<IOutputDevice>();
            outputDevice.IsAvailable.Returns(true);
            var inputs = new[] { buildId1 };
            var monitor = new TeamCityMonitor(inputDevice, d => connection);

            monitor.AddBuilds(outputDevice, inputs);

            await monitor.BeginMonitoring();

            await connection.Received().GetBuild(buildId1);
        }

        [Fact]
        public async void should_not_retrieve_builds_if_output_device_unavailable()
        {
            var inputDevice = Substitute.For<IInputDevice>();
            var connection = Substitute.For<ITeamCityConnection>();
            connection.GetBuild(Arg.Any<string>()).ReturnsForAnyArgs("<builds />");
            var outputDevice = Substitute.For<IOutputDevice>();
            outputDevice.IsAvailable.Returns(false);
            var inputs = new[] { buildId1 };
            var monitor = new TeamCityMonitor(inputDevice, d => connection);
            monitor.AddBuilds(outputDevice, inputs);

            await monitor.BeginMonitoring();

            await connection.DidNotReceiveWithAnyArgs().GetBuild(buildId1);
        }

        [Fact]
        public async void should_retrieve_builds_for_all_available_devices()
        {
            var inputDevice = Substitute.For<IInputDevice>();
            var connection = Substitute.For<ITeamCityConnection>();
            connection.GetBuild(Arg.Any<string>()).ReturnsForAnyArgs("<builds />");

            var outputDevice1 = Substitute.For<IOutputDevice>();
            outputDevice1.IsAvailable.Returns(true);
            var inputs1 = new[] { buildId1, buildId2 };

            var outputDevice2 = Substitute.For<IOutputDevice>();
            outputDevice2.IsAvailable.Returns(false);
            var inputs2 = new[] { buildId3 };

            var monitor = new TeamCityMonitor(inputDevice, d => connection);
            monitor.AddBuilds(outputDevice1, inputs1);
            monitor.AddBuilds(outputDevice2, inputs2);

            await monitor.BeginMonitoring();

            await Task.WhenAll(
                connection.Received().GetBuild(buildId1),
                connection.Received().GetBuild(buildId2),
                connection.DidNotReceive().GetBuild(buildId3));
        }
    }
}
