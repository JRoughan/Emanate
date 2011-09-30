using NUnit.Framework;

namespace Emanate.IntegrationTests.Service
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

        //[Test, Ignore("Need to find a way of getting app settings to support this test in R#")]
        //public void should()
        //{
        //    var service = new EmanateService();
        //    service.Start();
        //}
    }
}
