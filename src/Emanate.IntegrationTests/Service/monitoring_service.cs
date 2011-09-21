using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using Emanate.Core;
using Emanate.Core.Input;
using Emanate.Core.Input.TeamCity;
using Emanate.Service;
using NUnit.Framework;

namespace Emanate.IntegrationTests.Input
{
    [TestFixture]
    public class monitoring_service
    {
        //[TestFixtureSetUp]
        //public void SetUp()
        //{
        //    var originalFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
        //    var appConfigPath = Path.Combine(originalFolder, "App.Config").Replace("file:\\", "");

        //    if (!File.Exists(appConfigPath))
        //        throw new Exception("Could not find App.Config to use for integration tests.");

        //    AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", appConfigPath);
        //    typeof(ConfigurationManager).GetField("s_initState", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, 0);
        //}

        [Test, Ignore("Need to find a way of getting app settings to support this test in R#")]
        public void should()
        {
            var service = new MonitoringService();
            service.Start();
        }
    }
}
