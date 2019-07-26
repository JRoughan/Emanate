using System;
using System.Linq;
using Emanate.Core;
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
                var caretaker = new ConfigurationCaretakerBuilder()
                    .SetConfig(null)
                    .Build();

                var config = await caretaker.Load();

                Assert.NotNull(config);

                Assert.Empty(config.Mappings);

                Assert.Empty(config.InputModules);
                Assert.Empty(config.InputDevices);

                Assert.Empty(config.OutputModules);
                Assert.Empty(config.OutputDevices);
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
                Assert.Equal(Guid.Parse("30b0091d-6c5c-4460-8da7-8059a5461a41"), inputGroups.SourceDeviceId);
                var input = inputGroups.SourceConfiguration.Builds;
                Assert.Equal("MyInput", input);
            }
        }
    }
}
