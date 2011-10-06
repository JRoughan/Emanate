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

            Assert.That(monitor.MonitoredProjects.Contains("bt1"));
            Assert.That(monitor.MonitoredProjects.Contains("bt2"));
        }

        [Test, Ignore]
        public void should_match_projects_by_wildcard()
        {
            var connection = new MockTeamCityConnection("ProjectName1:BuildName1;ProjectName2:BuildName1");
            var configuration = new TeamCityConfiguration { BuildsToMonitor = "ProjectName*:BuildName1" };
            var monitor = new TeamCityMonitor(connection, configuration);

            monitor.BeginMonitoring();

            Assert.That(monitor.MonitoredProjects.Contains("bt1"));
            Assert.That(monitor.MonitoredProjects.Contains("bt2"));
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
    }

    internal class MockTeamCityConnection : ITeamCityConnection
    {
        private static int projectInstances;
        private static int buildInstances;

        private readonly Dictionary<ProjectInfo, List<BuildInfo>> projects = new Dictionary<ProjectInfo, List<BuildInfo>>();

        class ProjectInfo
        {
            public ProjectInfo(string name)
            {
                Name = name;
                Id = "project" + ++projectInstances;
            }

            public string Id { get; private set; }
            public string Name { get; private set; }

            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }

                return Id.Equals(((ProjectInfo)obj).Id);
                
            }

            public override int GetHashCode()
            {
                return Id.GetHashCode();
            }
        }

        class BuildInfo
        {
            public BuildInfo(string name)
            {
                Name = name;
                Id = "bt" + ++buildInstances;
                Status = "SUCCESS";
            }

            public string Id { get; private set; }
            public string Name { get; private set; }

            public string Status { get; set; }

            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }

                return Id.Equals(((ProjectInfo)obj).Id);

            }

            public override int GetHashCode()
            {
                return Id.GetHashCode();
            }
        }

        public MockTeamCityConnection(string projectsAndBuilds)
        {
            projectInstances = 0;
            buildInstances = 0;

            var pairParts = projectsAndBuilds.Split(new[] { ';' });
            foreach (var pairPart in pairParts)
            {
                var parts = pairPart.Split(new[] { ':' });

                var pi = new ProjectInfo(parts[0]);
                List<BuildInfo> builds;
                if (!projects.TryGetValue(pi, out builds))
                {
                    builds = new List<BuildInfo>();
                    projects.Add(pi, builds);
                }

                var buildNames = parts[1].Split(new[] { ',' });
                builds.AddRange(buildNames.Select(b => new BuildInfo(b)));
            }
        }

        public string GetProjects()
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>");
            sb.AppendLine(@"<projects>");
            foreach (var project in projects.Keys)
            {
                sb.AppendFormat(@"<project name=""{0}"" id=""{1}"" />{2}", project.Name, project.Id, Environment.NewLine);
            }
            sb.AppendLine(@"</projects>");
            return sb.ToString();
        }

        public string GetProject(string projectId)
        {
            var project = projects.Single(p => p.Key.Id == projectId);
            var sb = new StringBuilder();
            sb.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>");
            sb.AppendFormat(@"<project name=""{0}"" id=""{1}"">{2}", project.Key.Name, project.Key.Id, Environment.NewLine);
            sb.AppendLine(@"<buildTypes>");
            foreach (var build in project.Value)
            {
                sb.AppendFormat(@"<buildType id=""{0}"" name=""{1}"" />{2}", build.Id, build.Name, Environment.NewLine);
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

            // TODO: Simulate more than one build in history?
            var builds = projects.SelectMany(p => p.Value.Where(b => b.Id == buildId));
            foreach (var build in builds)
            {
                sb.AppendFormat(@"<build id=""999"" status=""{0}"" buildTypeId=""{1}"" /> {2}", build.Status, build.Id, Environment.NewLine);
            }
            sb.AppendLine(@"</builds>");
            return sb.ToString();
        }

        public void SetBuildStatus(string buildName, string status)
        {
            var builds = projects.SelectMany(p => p.Value.Where(b => b.Name == buildName));
            foreach (var build in builds)
            {
                build.Status = status;
            }
        }
    }
}
