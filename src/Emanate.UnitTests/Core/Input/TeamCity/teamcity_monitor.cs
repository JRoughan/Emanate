using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emanate.Core.Input;
using Emanate.Core.Input.TeamCity;
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

            Assert.That(monitor.MonitoredProjects.Contains("BuildName1id"));
            Assert.That(monitor.MonitoredProjects.Contains("BuildName2id"));
        }

        

        [Test]
        public void should_monitor_single_matching_project_if_multiple_projects_exist()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1;ProjectName2:BuildName2;ProjectName3:BuildName3");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.AreNotEqual(BuildState.Unknown, monitor.CurrentState);
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
            connection.SetBuildStatus("BuildName1id", "XXX");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName1:BuildName1" };
            var monitor = new TeamCityMonitor(connection, configuration);

            Assert.Throws<NotSupportedException>(monitor.BeginMonitoring);
        }
    }

    internal class MockTeamCityConnection : ITeamCityConnection
    {
        private readonly Dictionary<string, List<string>> projects = new Dictionary<string, List<string>>();
        private readonly Dictionary<string, string> buildStates = new Dictionary<string, string>();

        public MockTeamCityConnection(string projectsAndBuilds)
        {
            var pairParts = projectsAndBuilds.Split(new[] { ';' });
            foreach (var pairPart in pairParts)
            {
                var parts = pairPart.Split(new[] { ':' });
                List<string> builds;
                if (!projects.TryGetValue(parts[0], out builds))
                {
                    builds = new List<string>();
                    projects.Add(parts[0], builds);
                }

                builds.AddRange(parts[1].Split(new[] { ',' }));
            }
        }

        public string GetProjects()
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>");
            sb.AppendLine(@"<projects>");
            foreach (var project in projects.Keys)
            {
                sb.AppendFormat(@"<project name=""{0}"" id=""{1}"" />{2}", project, project + "id", Environment.NewLine);
            }
            sb.AppendLine(@"</projects>");
            return sb.ToString();
        }

        public string GetProject(string projectId)
        {
            var project = projects.Single(p => p.Key + "id" == projectId);
            var sb = new StringBuilder();
            sb.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>");
            sb.AppendFormat(@"<project name=""{0}"" id=""{1}"">{2}", project.Key, project.Key + "id", Environment.NewLine);
            sb.AppendLine(@"<buildTypes>");
            foreach (var build in project.Value)
            {
                sb.AppendFormat(@"<buildType id=""{0}"" name=""{1}"" />{2}", build + "id", build, Environment.NewLine);
            }
            sb.AppendLine(@"</buildTypes>");
            sb.AppendLine(@"</project>");
            return sb.ToString();
        }

        public string GetRunningBuilds()
        {
            return @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?><builds count=""0""></builds>";
        }

        public string GetBuild(string buildId)
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>");
            sb.AppendLine(@"<builds>");

            // TODO: Allow more than one build in history
            string status;
            if (!buildStates.TryGetValue(buildId, out status))
                status = "SUCCESS";
            sb.AppendFormat(@"<build id=""999"" status=""{0}"" buildTypeId=""{1}"" /> {2}", status, buildId, Environment.NewLine);

            sb.AppendLine(@"</builds>");
            return sb.ToString();
        }

        public void SetBuildStatus(string buildid, string status)
        {
            if (buildStates.ContainsKey(buildid))
                buildStates[buildid] = status;
            else
                buildStates.Add(buildid, status);
        }
    }
}
