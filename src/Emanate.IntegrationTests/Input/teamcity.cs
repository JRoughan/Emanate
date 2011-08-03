using System.Threading;
using Emanate.Core.Input.TeamCity;
using NUnit.Framework;

namespace Emanate.IntegrationTests
{
    [TestFixture]
    public class teamcity
    {
        [Test]
        public void should_get_all_projects()
        {
            var monitor = new TeamCityMonitor();
            var projects = monitor.GetAllProjects();

            Assert.IsTrue(projects.Contains("Lync PBX"));
        }
    }
}
