using System;
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
        public void should_monitor_single_project()
        {
            var connection = new Mock<ITeamCityConnection>();
            string projectsXml =
@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?> 
<projects>
  <project name=""ProjectName1"" id=""project1"" href=""/httpAuth/app/rest/projects/id:project1"" /> 
  <project name=""ProjectName2"" id=""project2"" href=""/httpAuth/app/rest/projects/id:project2"" /> 
  <project name=""ProjectName3"" id=""project3"" href=""/httpAuth/app/rest/projects/id:project3"" /> 
</projects>";
            string projectXml =
@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?> 
<project webUrl=""http://teamcityuri/project.html?projectId=project2"" description="""" archived=""false"" name=""ProjectName1"" id=""project1"" href=""/httpAuth/app/rest/projects/id:project1"">
  <buildTypes>
    <buildType id=""bt1"" name=""BuildName1"" href=""/httpAuth/app/rest/buildTypes/id:bt1"" projectName=""ProjectName1"" projectId=""project1"" webUrl=""http://teamcityuri/viewType.html?buildTypeId=bt1"" /> 
  </buildTypes>
</project>";
            string runningBuildsXml =
@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?><builds count=""0""></builds>";
            string buildXml =
@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?> 
<builds>
  <build id=""811"" number=""419"" status=""SUCCESS"" buildTypeId=""bt1"" startDate=""20110923T153954+1200"" href=""/httpAuth/app/rest/builds/id:811"" /> 
</builds>";
            connection.Setup(c => c.GetProjects()).Returns(projectsXml);
            connection.Setup(c => c.GetProject("project1")).Returns(projectXml);
            connection.Setup(c => c.GetRunningBuilds()).Returns(runningBuildsXml);
            connection.Setup(c => c.GetBuild("bt1")).Returns(buildXml);
            var configuration = new TeamCityConfiguration();
            configuration.BuildsToMonitor = "ProjectName1:BuildName1";
            var monitor = new TeamCityMonitor(connection.Object, configuration);

            monitor.BeginMonitoring();

            Assert.AreEqual(BuildState.Succeeded, monitor.CurrentState);
        }

        [Test]
        public void should_fail_if_build_configuration_missing()
        {
            var connection = new Mock<ITeamCityConnection>();
            string projectsXml =
@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<projects>
  <project name=""ProjectName1"" id=""project1"" href=""/httpAuth/app/rest/projects/id:project2"" /> 
</projects>";
            connection.Setup(c => c.GetProjects()).Returns(projectsXml);
            var configuration = new TeamCityConfiguration();
            configuration.BuildsToMonitor = "ProjectName1";
            var monitor = new TeamCityMonitor(connection.Object, configuration);

            Assert.Throws<IndexOutOfRangeException>(monitor.BeginMonitoring);
        }
    }
}
