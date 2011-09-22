using Emanate.Core;
using Emanate.Core.Input.TeamCity;
using Moq;
using NUnit.Framework;

namespace Emanate.UnitTests.Core
{
    [TestFixture]
    public class reflection_configuration_generator
    {
        [Test]
        public void should_populate_string_properties()
        {
            var storage = new Mock<IConfigurationStorage>();
            storage.Setup(s => s.GetString("Key1")).Returns("MyValue");

            var configurationBuilder = new ReflectionConfigurationGenerator((storage.Object));
            var config = configurationBuilder.Generate<ValidConfig>();

            Assert.AreEqual("MyValue", config.Value1);
        }
    }

    class ValidConfig
    {
        [Key("Key1")]
        public string Value1 { get; set; }
    }
}
