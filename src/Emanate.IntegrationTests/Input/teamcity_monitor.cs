using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using Emanate.Core;
using Emanate.Core.Input.TeamCity;
using NUnit.Framework;

namespace Emanate.IntegrationTests.Input
{
    [TestFixture]
    public class teamcity_monitor
    {
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
        public void should_get_all_projects()
        {
            var configuration = new ApplicationConfiguration();
            var monitor = new TeamCityMonitor(configuration);
            var projects = monitor.GetProjects();

            Assert.IsTrue(projects.Contains("Lync PBX"));
        }
    }
}
