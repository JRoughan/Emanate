using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emanate.Core.Input.TeamCity;

namespace Emanate.UnitTests.Core.Input.TeamCity
{
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

            public bool IsRunning { get; set; }

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

        public string GetBuild(string buildId)
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>");
            sb.AppendLine(@"<builds>");

            // TODO: Simulate more than one build in history?
            var builds = projects.SelectMany(p => p.Value.Where(b => b.Id == buildId));
            foreach (var build in builds)
            {
                var runningXml = build.IsRunning ? @"running=""true"" percentageComplete=""35""" : "";
                sb.AppendFormat(@"<build id=""999"" {0} status=""{1}"" buildTypeId=""{2}"" /> {3}", runningXml, build.Status, build.Id, Environment.NewLine);
            }
            sb.AppendLine(@"</builds>");
            return sb.ToString();
        }

        public void SetBuildStatus(string buildName, string status, bool isRunning = false)
        {
            var builds = projects.SelectMany(p => p.Value.Where(b => b.Name == buildName));
            foreach (var build in builds)
            {
                build.Status = status;
                build.IsRunning = isRunning;
            }
        }
    }
}