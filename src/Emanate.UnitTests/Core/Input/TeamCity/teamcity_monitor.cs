using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Timers;
using Emanate.Core.Input;
using Emanate.Core.Input.TeamCity;
using Moq;
using NUnit.Framework;
using Timer = System.Timers.Timer;

namespace Emanate.UnitTests.Core.Input.TeamCity
{
    [TestFixture]
    public class teamcity_monitor
    {
        // TODO: Ignore dupe monitored builds

        [Test]
        public void should_not_use_connection_until_monitoring_starts()
        {
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1" };

            Assert.DoesNotThrow(() => new TeamCityMonitor(null, configuration));
        }

        [Test]
        public void should_ignore_leading_spaces_for_projects()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1;ProjectName1:BuildName2");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1; ProjectName1:BuildName2" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.That(monitor.MonitoredBuilds.Contains("bt1"));
            Assert.That(monitor.MonitoredBuilds.Contains("bt2"));
        }

        [Test]
        public void should_ignore_trailing_spaces_for_projects()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1;ProjectName1:BuildName2");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1;ProjectName1 :BuildName2" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.That(monitor.MonitoredBuilds.Contains("bt1"));
            Assert.That(monitor.MonitoredBuilds.Contains("bt2"));
        }

        [Test]
        public void should_ignore_leading_spaces_for_builds()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1;ProjectName1:BuildName2");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1;ProjectName1: BuildName2" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.That(monitor.MonitoredBuilds.Contains("bt1"));
            Assert.That(monitor.MonitoredBuilds.Contains("bt2"));
        }

        [Test]
        public void should_ignore_trailing_spaces_for_builds()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1;ProjectName1:BuildName2");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1;ProjectName1:BuildName2 " };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.That(monitor.MonitoredBuilds.Contains("bt1"));
            Assert.That(monitor.MonitoredBuilds.Contains("bt2"));
        }

        [Test]
        public void should_not_fail_if_no_builds_match()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "NotProjectName1:NotBuildName1" };
            var monitor = new TeamCityMonitor(connection, configuration);

            Assert.DoesNotThrow(monitor.BeginMonitoring);
        }

        [Test]
        public void should_monitor_single_project()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.AreEqual(BuildState.Succeeded, monitor.CurrentState);
        }

        [Test]
        public void should_monitor_multiple_projects()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1;ProjectName2:BuildName2");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1;ProjectName2:BuildName2" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.That(monitor.MonitoredBuilds.Contains("bt1"));
            Assert.That(monitor.MonitoredBuilds.Contains("bt2"));
        }

        [Test]
        public void should_match_projects_by_wildcard()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName*:BuildName1" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.That(monitor.MonitoredBuilds.Contains("bt1"));
        }

        [Test]
        public void should_match_multiple_projects_by_wildcard()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1;ProjectName2:BuildName1");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName*:BuildName1" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.That(monitor.MonitoredBuilds.Contains("bt1"));
            Assert.That(monitor.MonitoredBuilds.Contains("bt2"));
        }

        [Test]
        public void should_only_monitor_matches_when_using_wildcard_for_projects()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1;ProjectName2:BuildName1;NotProjectName:BuildName1");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName*:BuildName1" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.That(monitor.MonitoredBuilds.Contains("bt1"));
            Assert.That(monitor.MonitoredBuilds.Contains("bt2"));
            Assert.That(!monitor.MonitoredBuilds.Contains("bt3"));
        }

        [Test]
        public void should_match_build_by_wildcard()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName*:BuildName1" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.That(monitor.MonitoredBuilds.Contains("bt1"));
        }

        [Test]
        public void should_match_multiple_builds_by_wildcard()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1;ProjectName1:BuildName2;ProjectName1:BuildName3");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:Build*" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.That(monitor.MonitoredBuilds.Contains("bt1"));
            Assert.That(monitor.MonitoredBuilds.Contains("bt2"));
            Assert.That(monitor.MonitoredBuilds.Contains("bt3"));
        }

        [Test]
        public void should_only_monitor_matches_when_using_wildcard_for_builds()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1;ProjectName1:BuildName2;ProjectName2:NotBuildName3");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:Build*" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.That(monitor.MonitoredBuilds.Contains("bt1"));
            Assert.That(monitor.MonitoredBuilds.Contains("bt2"));
            Assert.That(!monitor.MonitoredBuilds.Contains("bt3"));
        }

        [Test]
        public void should_match_project_and_build_by_wildcard()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName*:BuildName*" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.That(monitor.MonitoredBuilds.Contains("bt1"));
        }

        [Test]
        public void should_match_multiple_projects_and_builds_by_wildcard()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1;ProjectName2:BuildName2");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName*:BuildName*" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.That(monitor.MonitoredBuilds.Contains("bt1"));
            Assert.That(monitor.MonitoredBuilds.Contains("bt2"));
        }

        [Test]
        public void should_only_monitor_matches_when_using_wildcard_for_projects_and_builds()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1;ProjectName2:BuildName2;NotProjectName:BuildName2;ProjectName3:NotBuildName;NotProjectName:NotBuildName");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName*:BuildName*" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.That(monitor.MonitoredBuilds.Contains("bt1"));
            Assert.That(monitor.MonitoredBuilds.Contains("bt2"));
            Assert.That(!monitor.MonitoredBuilds.Contains("bt3"));
            Assert.That(!monitor.MonitoredBuilds.Contains("bt4"));
            Assert.That(!monitor.MonitoredBuilds.Contains("bt5"));
        }

        [Test]
        public void should_only_monitor_matching_projects()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1;ProjectName2:BuildName2;ProjectName3:BuildName3");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.That(monitor.MonitoredBuilds.Contains("bt1"));
            Assert.That(!monitor.MonitoredBuilds.Contains("bt2"));
            Assert.That(!monitor.MonitoredBuilds.Contains("bt3"));
        }

        [Test]
        public void should_fail_if_build_missing_in_config()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1" };
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
        public void should_stop_polling_when_end_monitoring_called()
        {
            int buildChecks = 0;
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1");
            var mockConnection = new Mock<ITeamCityConnection>();
            mockConnection.Setup(c => c.GetProjects()).Returns(connection.GetProjects);
            mockConnection.Setup(c => c.GetProject(It.IsAny<string>())).Returns<string>(connection.GetProject);
            mockConnection.Setup(c => c.GetBuild(It.IsAny<string>())).Callback(() => buildChecks++).Returns<string>(connection.GetBuild);
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
            var firstUpdate = true;
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1");
            var mockConnection = new Mock<ITeamCityConnection>();
            mockConnection.Setup(c => c.GetProjects()).Returns(connection.GetProjects);
            mockConnection.Setup(c => c.GetProject(It.IsAny<string>())).Returns<string>(connection.GetProject);
            mockConnection.Setup(c => c.GetBuild(It.IsAny<string>()))
                .Callback(() => { if (!firstUpdate) { Thread.Sleep(1000); } firstUpdate = false; })
                .Returns<string>(connection.GetBuild);
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1", PollingInterval = 1 };
            var monitor = new TeamCityMonitor(mockConnection.Object, configuration);
            var timerField = monitor.GetType().GetField("timer", BindingFlags.NonPublic | BindingFlags.Instance);
            var timer = (Timer)timerField.GetValue(monitor);
            timer.Interval = 1;
            timerField.SetValue(monitor, timer);
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

        [Test]
        public void should_raise_event_when_a_build_status_changes()
        {
            bool statusChangedCalled = false;
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1", PollingInterval = 1 };
            var monitor = new TeamCityMonitor(connection, configuration);
            monitor.StatusChanged += (s, e) => statusChangedCalled = true;

            monitor.BeginMonitoring();

            var sw = new Stopwatch();
            sw.Start();
            do { Thread.Sleep(500); }
            while (!statusChangedCalled && sw.ElapsedMilliseconds < 5000);

            Assert.IsTrue(statusChangedCalled);
        }

        [Test]
        public void should_set_status_succeeded_if_single_build_succeeds()
        {
            var buildStatus = BuildState.Unknown;
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1" };
            var monitor = new TeamCityMonitor(connection, configuration);
            monitor.StatusChanged += (s, e) => buildStatus = e.NewState;

            monitor.BeginMonitoring();

            Assert.AreEqual(BuildState.Succeeded, buildStatus);
        }

        [Test]
        public void should_set_status_failed_if_single_build_fails()
        {
            var buildStatus = BuildState.Unknown;
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1");
            connection.SetBuildStatus("BuildName1", "FAILURE");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1" };
            var monitor = new TeamCityMonitor(connection, configuration);
            monitor.StatusChanged += (s, e) => buildStatus = e.NewState;

            monitor.BeginMonitoring();

            Assert.AreEqual(BuildState.Failed, buildStatus);
        }

        [Test]
        public void should_set_status_succeeded_if_all_builds_succeed()
        {
            var buildStatus = BuildState.Unknown;
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1;ProjectName2:BuildName2;ProjectName3:BuildName3");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1;ProjectName2:BuildName2;ProjectName3:BuildName3" };
            var monitor = new TeamCityMonitor(connection, configuration);
            monitor.StatusChanged += (s, e) => buildStatus = e.NewState;

            monitor.BeginMonitoring();

            Assert.AreEqual(BuildState.Succeeded, buildStatus);
        }

        [Test]
        public void should_set_status_failed_if_any_build_fails()
        {
            var buildStatus = BuildState.Unknown;
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1;ProjectName2:BuildName2;ProjectName3:BuildName3");
            connection.SetBuildStatus("BuildName1", "FAILURE");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1;ProjectName2:BuildName2;ProjectName3:BuildName3" };
            var monitor = new TeamCityMonitor(connection, configuration);
            monitor.StatusChanged += (s, e) => buildStatus = e.NewState;

            monitor.BeginMonitoring();

            Assert.AreEqual(BuildState.Failed, buildStatus);
        }


        [Test]
        public void should_return_success_if_build_is_successful()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1");
            connection.SetBuildStatus("BuildName1", "SUCCESS");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.AreEqual(BuildState.Succeeded, monitor.CurrentState);
        }

        [Test]
        public void should_return_failure_if_build_has_failed()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1");
            connection.SetBuildStatus("BuildName1", "FAILURE");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.AreEqual(BuildState.Failed, monitor.CurrentState);
        }

        [Test]
        public void should_return_unknown_if_there_are_no_matching_builds()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName2" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.AreEqual(BuildState.Unknown, monitor.CurrentState);
        }

        [Test]
        public void should_return_unknown_if_build_is_removed()
        {
            int buildChecks = 0;
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1");
            var mockConnection = new Mock<ITeamCityConnection>();
            mockConnection.Setup(c => c.GetProjects()).Returns(connection.GetProjects);
            mockConnection.Setup(c => c.GetProject(It.IsAny<string>())).Returns<string>(connection.GetProject);
            mockConnection.Setup(c => c.GetBuild(It.IsAny<string>())).Callback(() => buildChecks++).Returns<string>(connection.GetBuild);
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1", PollingInterval = 1 };
            var monitor = new TeamCityMonitor(mockConnection.Object, configuration);
            monitor.BeginMonitoring();

            connection.RemoveAllBuilds();
            var checksBeforeRemove = buildChecks;

            do { Thread.Sleep(100); }
            while (buildChecks == checksBeforeRemove);

            Assert.AreEqual(BuildState.Unknown, monitor.CurrentState);
        }

        [Test]
        public void should_return_running_if_build_is_still_running_and_successful_so_far()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1");
            connection.SetBuildStatus("BuildName1", "SUCCESS", true);
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.AreEqual(BuildState.Running, monitor.CurrentState);
        }

        [Test]
        public void should_return_failure_if_build_is_still_running_but_has_failed()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1");
            connection.SetBuildStatus("BuildName1", "FAILURE", true);
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.AreEqual(BuildState.Failed, monitor.CurrentState);
        }

        [Test]
        public void should_return_success_if_all_builds_successful()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1;ProjectName2:BuildName2");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1;ProjectName2:BuildName2" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.AreEqual(BuildState.Succeeded, monitor.CurrentState);
        }

        [Test]
        public void should_return_failure_if_all_builds_failed()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1;ProjectName2:BuildName2");
            connection.SetBuildStatus("BuildName1", "FAILURE");
            connection.SetBuildStatus("BuildName2", "FAILURE");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1;ProjectName2:BuildName2" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.AreEqual(BuildState.Failed, monitor.CurrentState);
        }

        [Test]
        public void should_return_error_if_any_build_is_in_error()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1;ProjectName2:BuildName2");
            connection.SetBuildStatus("BuildName1", "SUCCESS");
            connection.SetBuildStatus("BuildName2", "ERROR");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1;ProjectName2:BuildName2" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.AreEqual(BuildState.Error, monitor.CurrentState);
        }

        [Test]
        public void should_return_error_if_all_builds_are_in_error()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1;ProjectName2:BuildName2");
            connection.SetBuildStatus("BuildName1", "ERROR");
            connection.SetBuildStatus("BuildName2", "ERROR");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1;ProjectName2:BuildName2" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.AreEqual(BuildState.Error, monitor.CurrentState);
        }

        [Test]
        public void should_return_failure_if_any_build_failed()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1;ProjectName2:BuildName2");
            connection.SetBuildStatus("BuildName1", "SUCCESS");
            connection.SetBuildStatus("BuildName2", "FAILURE");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1;ProjectName2:BuildName2" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.AreEqual(BuildState.Failed, monitor.CurrentState);
        }

        [Test]
        public void should_return_running_if_any_build_running_when_others_failed()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1;ProjectName2:BuildName2");
            connection.SetBuildStatus("BuildName1", "SUCCESS", true);
            connection.SetBuildStatus("BuildName2", "FAILURE");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1;ProjectName2:BuildName2" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.AreEqual(BuildState.Running, monitor.CurrentState);
        }

        [Test]
        public void should_return_running_if_any_build_running_when_others_succeeded()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1;ProjectName2:BuildName2");
            connection.SetBuildStatus("BuildName1", "SUCCESS", true);
            connection.SetBuildStatus("BuildName2", "SUCCESS");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1;ProjectName2:BuildName2" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.AreEqual(BuildState.Running, monitor.CurrentState);
        }

        [Test]
        public void should_only_get_project_info_once_per_project()
        {
            int numberOfCalls = 0;
            var fakeValueProvider = new MockTeamCityConnection("ProjectName1:BuildName1;ProjectName1:BuildName2");
            var connection = new Mock<ITeamCityConnection>();
            connection.Setup(c => c.GetBuild(It.IsAny<string>())).Returns<string>(fakeValueProvider.GetBuild);
            connection.Setup(c => c.GetProjects()).Returns(fakeValueProvider.GetProjects);
            connection.Setup(c => c.GetProject(It.IsAny<string>())).Callback(() => numberOfCalls++).Returns<string>(fakeValueProvider.GetProject);
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1;ProjectName1:BuildName2" };
            var monitor = new TeamCityMonitor(connection.Object, configuration);

            monitor.BeginMonitoring();

            Assert.AreEqual(1, numberOfCalls);
        }

        [Test]
        public void should_ignore_duplicate_builds()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1;ProjectName1:BuildName1" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.AreEqual(1, monitor.MonitoredBuilds.Count());
        }
    }
}
