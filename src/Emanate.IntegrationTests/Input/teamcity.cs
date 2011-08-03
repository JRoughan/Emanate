using Emanate.Core;
using Emanate.Core.Input.TeamCity;
using Moq;
using NUnit.Framework;

namespace Emanate.IntegrationTests.Input
{
    [TestFixture]
    public class teamcity
    {
        [Test]
        public void should_get_all_projects()
        {
            var monitor = new TeamCityMonitor(CreateValidConfiguration());
            var projects = monitor.GetProjects();

            Assert.IsTrue(projects.Contains("Lync PBX"));
        }

        private IConfiguration CreateValidConfiguration()
        {
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(c => c.GetString("Host")).Returns("xxx");
            configuration.Setup(c => c.GetBool("IsSSL")).Returns(false);
            configuration.Setup(c => c.GetString("User")).Returns("xxx");
            configuration.Setup(c => c.GetString("Password")).Returns("xxx");
            return configuration.Object;
        }
    }
}
