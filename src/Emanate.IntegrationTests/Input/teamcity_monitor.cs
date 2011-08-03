using System.IO;
using System.Xml.Serialization;
using Emanate.Core;
using Emanate.Core.Input.TeamCity;
using Moq;
using NUnit.Framework;

namespace Emanate.IntegrationTests.Input
{
    [TestFixture]
    public class teamcity_monitor
    {
        [Test]
        public void should_get_all_projects()
        {
            var monitor = new TeamCityMonitor(CreateValidConfiguration());
            var projects = monitor.GetProjects();

            Assert.IsTrue(projects.Contains("Lync PBX"));
        }

        private static IConfiguration CreateValidConfiguration()
        {
            SecurityInfo securityInfo;
            var deSerializer = new XmlSerializer(typeof(SecurityInfo));
            using (var stream = File.OpenRead("Security.config"))
                securityInfo = (SecurityInfo)deSerializer.Deserialize(stream);

            var configuration = new Mock<IConfiguration>();
            configuration.Setup(c => c.GetString("Host")).Returns(securityInfo.TeamCityUri);
            configuration.Setup(c => c.GetBool("IsSSL")).Returns(false);
            configuration.Setup(c => c.GetString("User")).Returns(securityInfo.TeamCityUser);
            configuration.Setup(c => c.GetString("Password")).Returns(securityInfo.TeamCityPassword);
            return configuration.Object;
        }
    }
}
