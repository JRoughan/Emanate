using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using Emanate.Core.Input;
using Emanate.TeamCity;
using NUnit.Framework;

namespace Emanate.IntegrationTests.Core.Input
{
    [TestFixture]
    public class teamcity_monitor
    {
        private string teamCityUri = "http://teamcityuri";

        [TestFixtureSetUp]
        public void SetUp()
        {
            var originalFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            var appConfigPath = Path.Combine(originalFolder, "App.Config").Replace("file:\\", "");

            if (!File.Exists(appConfigPath))
                throw new Exception("Could not find App.Config to use for integration tests.");

            AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", appConfigPath);
            typeof(ConfigurationManager).GetField("s_initState", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, 0);
        }

        [Test]
        public void should_be_in_unknown_state_before_started()
        {
            var device = new TeamCityDevice { Uri = "http://dummy", PollingInterval = 60 };
            var monitor = new TeamCityMonitor(null, device);

            Assert.AreEqual(BuildState.Unknown, monitor.CurrentState);
        }

        [Test]
        public void should_run_update_when_started()
        {
            var device = new TeamCityDevice { Uri = teamCityUri, PollingInterval = 60 };
            var monitor = new TeamCityMonitor(null, device);

            //monitor.BeginMonitoring();

            Assert.AreNotEqual(BuildState.Unknown, monitor.CurrentState);
        }
    }
}
