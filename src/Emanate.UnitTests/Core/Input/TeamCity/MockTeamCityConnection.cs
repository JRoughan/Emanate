using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emanate.Core.Input.TeamCity;

namespace Emanate.UnitTests.Core.Input.TeamCity
{
    internal class MockTeamCityConnection : ITeamCityConnection
    {
        private readonly int projectInstances;
        private int buildInstances;

        private readonly Dictionary<ProjectInfo, List<BuildInfo>> projects = new Dictionary<ProjectInfo, List<BuildInfo>>();

        class ProjectInfo
        {
            public ProjectInfo(string name, int id)
            {
                Name = name;
                Id = "project" + id;
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
            public BuildInfo(string name, int id)
            {
                Name = name;
                Id = "bt" + id;
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

                return Id.Equals(((BuildInfo)obj).Id);

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

                var projectName = parts[0];
                var projectInfo = projects.Select(kvp => kvp.Key).SingleOrDefault(p => p.Name == projectName) ?? new ProjectInfo(projectName, ++projectInstances);

                List<BuildInfo> builds;
                if (!projects.TryGetValue(projectInfo, out builds))
                {
                    builds = new List<BuildInfo>();
                    projects.Add(projectInfo, builds);
                }

                var buildNames = parts[1].Split(new[] { ',' });
                var newBuildNames = buildNames.Except(builds.Select(x => x.Name));
                builds.AddRange(newBuildNames.Select(b => new BuildInfo(b, ++buildInstances)));
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

            var builds = projects.SelectMany(p => p.Value.Where(b => b.Id == buildId));
            foreach (var build in builds)
            {
                var runningXml = build.IsRunning ? @"running=""true"" percentageComplete=""35""" : "";
                sb.AppendFormat(@"<build id=""999"" {0} status=""{1}"" buildTypeId=""{2}"" startDate=""20000101T120000+1300"" /> {3}", runningXml, build.Status, build.Id, Environment.NewLine);
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

        public void RemoveAllBuilds()
        {
            foreach (var project in projects)
            {
                project.Value.Clear();
            }
        }
    }
}