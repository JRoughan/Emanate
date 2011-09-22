using System;
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
            var configurationBuilder = new ReflectionConfigurationGenerator(storage.Object);

            var config = configurationBuilder.Generate<StringValueConfig>();

            Assert.AreEqual("MyValue", config.Value1);
        }

        [Test]
        public void should_populate_integer_properties()
        {
            var storage = new Mock<IConfigurationStorage>();
            storage.Setup(s => s.GetInt("Key1")).Returns(555);
            var configurationBuilder = new ReflectionConfigurationGenerator(storage.Object);

            var config = configurationBuilder.Generate<IntegerValueConfig>();

            Assert.AreEqual(555, config.Value1);
        }

        [Test]
        public void should_populate_boolean_properties()
        {
            var storage = new Mock<IConfigurationStorage>();
            storage.Setup(s => s.GetBool("Key1")).Returns(true);
            var configurationBuilder = new ReflectionConfigurationGenerator(storage.Object);

            var config = configurationBuilder.Generate<BooleanValueConfig>();

            Assert.AreEqual(true, config.Value1);
        }

        [Test]
        public void should_fail_if_key_missing()
        {
            var configurationBuilder = new ReflectionConfigurationGenerator(null);
            
            Assert.Throws<MissingKeyException>(() => configurationBuilder.Generate<MissingKeyConfig>());
        }

        [Test]
        public void should_fail_if_property_type_is_unsupported()
        {
            var configurationBuilder = new ReflectionConfigurationGenerator(null);

            Assert.Throws<NotSupportedException>(() => configurationBuilder.Generate<UnsupportedValueConfig>());
        }
    }

    class StringValueConfig
    {
        [Key("Key1")]
        public string Value1 { get; set; }
    }

    class IntegerValueConfig
    {
        [Key("Key1")]
        public int Value1 { get; set; }
    }

    class BooleanValueConfig
    {
        [Key("Key1")]
        public bool Value1 { get; set; }
    }

    class UnsupportedValueConfig
    {
        [Key("Key1")]
        public DateTime Value1 { get; set; }
    }

    class MissingKeyConfig
    {
        public string Value1 { get; set; }
    }
}
