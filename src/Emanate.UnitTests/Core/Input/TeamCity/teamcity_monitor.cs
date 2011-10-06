using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Emanate.Core.Input;
using Emanate.Core.Input.TeamCity;
using Moq;
using NUnit.Framework;

namespace Emanate.UnitTests.Core.Input.TeamCity
{
    [TestFixture]
    public class teamcity_monitor
    {
        [Test]
        public void should_not_use_connection_until_monitoring_starts()
        {
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1" };

            Assert.DoesNotThrow(() => new TeamCityMonitor(null, configuration));
        }

        [Test]
        public void should_monitor_single_project()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.AreNotEqual(BuildState.Unknown, monitor.CurrentState);
        }

        [Test]
        public void should_monitor_multiple_projects()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1;ProjectName2:BuildName2");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1;ProjectName2:BuildName2" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.That(monitor.MonitoredProjects.Contains("bt1"));
            Assert.That(monitor.MonitoredProjects.Contains("bt2"));
        }

        [Test]
        public void should_match_projects_by_wildcard()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName*:BuildName1" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.That(monitor.MonitoredProjects.Contains("bt1"));
        }

        [Test]
        public void should_match_multiple_projects_by_wildcard()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1;ProjectName2:BuildName1");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName*:BuildName1" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.That(monitor.MonitoredProjects.Contains("bt1"));
            Assert.That(monitor.MonitoredProjects.Contains("bt2"));
        }

        [Test]
        public void should_only_monitor_matches_when_using_wildcard_for_projects()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1;ProjectName1:BuildName1;NotProjectName2:BuildName1");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName*:BuildName1" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.That(monitor.MonitoredProjects.Contains("bt1"));
            Assert.That(monitor.MonitoredProjects.Contains("bt2"));
            Assert.That(!monitor.MonitoredProjects.Contains("bt3"));
        }

        [Test]
        public void should_match_build_by_wildcard()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName*:BuildName1" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.That(monitor.MonitoredProjects.Contains("bt1"));
        }

        [Test]
        public void should_match_multiple_builds_by_wildcard()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1;ProjectName1:BuildName2;ProjectName1:BuildName3");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:Build*" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.That(monitor.MonitoredProjects.Contains("bt1"));
            Assert.That(monitor.MonitoredProjects.Contains("bt2"));
            Assert.That(monitor.MonitoredProjects.Contains("bt3"));
        }

        [Test]
        public void should_only_monitor_matches_when_using_wildcard_for_builds()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1;ProjectName1:BuildName2;ProjectName2:NotBuildName3");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:Build*" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.That(monitor.MonitoredProjects.Contains("bt1"));
            Assert.That(monitor.MonitoredProjects.Contains("bt2"));
            Assert.That(!monitor.MonitoredProjects.Contains("bt3"));
        }

        [Test]
        public void should_match_project_and_build_by_wildcard()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName*:BuildName*" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.That(monitor.MonitoredProjects.Contains("bt1"));
        }

        [Test]
        public void should_match_multiple_projects_and_builds_by_wildcard()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1;ProjectName2:BuildName2");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName*:BuildName*" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.That(monitor.MonitoredProjects.Contains("bt1"));
            Assert.That(monitor.MonitoredProjects.Contains("bt2"));
        }

        [Test]
        public void should_only_monitor_matches_when_using_wildcard_for_projects_and_builds()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1;ProjectName2:BuildName2;NotProjectName:BuildName2;ProjectName3:NotBuildName;NotProjectName:NotBuildName");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName*:BuildName*" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.That(monitor.MonitoredProjects.Contains("bt1"));
            Assert.That(monitor.MonitoredProjects.Contains("bt2"));
            Assert.That(!monitor.MonitoredProjects.Contains("bt3"));
            Assert.That(!monitor.MonitoredProjects.Contains("bt4"));
            Assert.That(!monitor.MonitoredProjects.Contains("bt5"));
        }

        [Test]
        public void should_only_monitor_matching_projects()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1;ProjectName2:BuildName2;ProjectName3:BuildName3");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.That(monitor.MonitoredProjects.Contains("bt1"));
            Assert.That(!monitor.MonitoredProjects.Contains("bt2"));
            Assert.That(!monitor.MonitoredProjects.Contains("bt3"));
        }

        [Test]
        public void should_fail_if_build_missing_in_config()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1");
            var configuration = new TeamCityConfiguration();
            configuration.BuildsToMonitor = "ProjectName1";
            var monitor = new TeamCityMonitor(connection, configuration);

            Assert.Throws<IndexOutOfRangeException>(monitor.BeginMonitoring);
        }

        [Test]
        public void should_fail_if_build_status_unknown()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1");
            connection.SetBuildStatus("BuildName1", "XXX");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1" };
            var monitor = new TeamCityMonitor(connection, configuration);

            Assert.Throws<NotSupportedException>(monitor.BeginMonitoring);
        }

        [Test]
        public void should_poll_for_updates()
        {
            int buildChecks = 0;
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1");
            var mockConnection = new Mock<ITeamCityConnection>();
            mockConnection.Setup(c => c.GetProjects()).Returns(connection.GetProjects);
            mockConnection.Setup(c => c.GetProject(It.IsAny<string>())).Returns<string>(connection.GetProject);
            mockConnection.Setup(c => c.GetBuild(It.IsAny<string>())).Callback(() => buildChecks++).Returns<string>(connection.GetBuild);
            mockConnection.Setup(c => c.GetRunningBuilds()).Returns(connection.GetRunningBuilds);
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1", PollingInterval = 1 };
            var monitor = new TeamCityMonitor(mockConnection.Object, configuration);

            monitor.BeginMonitoring();

            var sw = new Stopwatch();
            sw.Start();
            do { Thread.Sleep(500); }
            while (buildChecks < 2 && sw.ElapsedMilliseconds < 5000);

            Assert.GreaterOrEqual(2, buildChecks);
        }

        [Test]
        public void should_stop_polling_when_end_monitoring_calls()
        {
            int buildChecks = 0;
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1");
            var mockConnection = new Mock<ITeamCityConnection>();
            mockConnection.Setup(c => c.GetProjects()).Returns(connection.GetProjects);
            mockConnection.Setup(c => c.GetProject(It.IsAny<string>())).Returns<string>(connection.GetProject);
            mockConnection.Setup(c => c.GetBuild(It.IsAny<string>())).Callback(() => buildChecks++).Returns<string>(connection.GetBuild);
            mockConnection.Setup(c => c.GetRunningBuilds()).Returns(connection.GetRunningBuilds);
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1", PollingInterval = 1 };
            var monitor = new TeamCityMonitor(mockConnection.Object, configuration);

            monitor.BeginMonitoring();

            var sw = new Stopwatch();
            sw.Start();
            do { Thread.Sleep(500); }
            while (buildChecks < 2 && sw.ElapsedMilliseconds < 5000);
            sw.Stop();

            var numberOfChecks = buildChecks;

            monitor.EndMonitoring();

            sw.Start();
            do { Thread.Sleep(500); }
            while (sw.ElapsedMilliseconds < 5000);
            sw.Stop();

            Assert.AreEqual(numberOfChecks, buildChecks);
        }

        [Test]
        public void should_not_fail_if_processing_time_longer_than_polling_interval()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1");
            var mockConnection = new Mock<ITeamCityConnection>();
            mockConnection.Setup(c => c.GetProjects()).Returns(connection.GetProjects);
            mockConnection.Setup(c => c.GetProject(It.IsAny<string>())).Returns<string>(connection.GetProject);
            mockConnection.Setup(c => c.GetBuild(It.IsAny<string>())).Callback(() => Thread.Sleep(1500)).Returns<string>(connection.GetBuild);
            mockConnection.Setup(c => c.GetRunningBuilds()).Returns(connection.GetRunningBuilds);
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1", PollingInterval = 1 };
            var monitor = new TeamCityMonitor(mockConnection.Object, configuration);

            monitor.BeginMonitoring();

            var sw = new Stopwatch();
            sw.Start();
            do { Thread.Sleep(500); }
            while (sw.ElapsedMilliseconds < 5000);
        }

        [Test]
        public void should_ignore_calls_to_end_monitoring_if_not_currently_running()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1" };
            var monitor = new TeamCityMonitor(connection, configuration);

            Assert.DoesNotThrow(monitor.EndMonitoring);
        }
    }
}
