using System;
using System.Linq;
using System.Xml.Linq;
using Emanate.Core;
using Emanate.Core.Configuration;
using NSubstitute;
using Xunit;

namespace Emanate.UnitTests.Core.Configuration
{
    public class configuration_caretaker
    {
        public class when_no_config_file_exists
        {
            [Fact]
            public async void should_return_empty_configuration()
            {
                var diskAccessor = Substitute.For<IDiskAccessor>();
                diskAccessor.Load(Arg.Any<string>()).Returns((XDocument)null);
                var caretaker = new ConfigurationCaretakerBuilder()
                    .SetConfig(null)
                    .Build();

                var config = await caretaker.Load();

                Assert.NotNull(config);

                Assert.Collection(config.Mappings);

                Assert.Collection(config.InputModules);
                Assert.Collection(config.InputDevices);
                Assert.Collection(config.InputConfigurations);

                Assert.Collection(config.OutputModules);
                Assert.Collection(config.OutputDevices);
                Assert.Collection(config.OutputConfigurations);
            }
        }

        public class when_complete_config_exists
        {
            [Fact]
            public async void should_return_complete_configuration()
            {
                var caretaker = new ConfigurationCaretakerBuilder()
                    .SetConfig(ConfigurationSamples.Complete)
                    .AddModule("module1", Direction.Input)
                    .AddModule("module2", Direction.Output)
                    .Build();

                var config = await caretaker.Load();

                Assert.Single(config.InputModules, m => m.Key == "module1");
                Assert.Single(config.OutputModules, m => m.Key == "module2");

                var mapping = config.Mappings.Single();
                Assert.Equal(Guid.Parse("59e491aa-58cc-4a50-b6af-0975d8708833"), mapping.OutputDeviceId);
                var inputGroups = mapping.InputGroups.Single();
                Assert.Equal(Guid.Parse("30b0091d-6c5c-4460-8da7-8059a5461a41"), inputGroups.InputDeviceId);
                var input = inputGroups.Inputs.Single();
                Assert.Equal("MyInput", input);
            }
        }
    }
}
