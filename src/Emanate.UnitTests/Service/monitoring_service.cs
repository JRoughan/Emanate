//using Autofac.Features.Indexed;
//using Emanate.Core;
//using Emanate.Core.Configuration;
//using Emanate.Core.Input;
//using Emanate.Core.Output;
//using Emanate.UnitTests.Builders;
//using NSubstitute;
//using Xunit;

//namespace Emanate.UnitTests.Service
//{
//    public class monitoring_service
//    {
//        [Fact]
//        public void should_not_begin_monitoring_when_started_if_no_mapping()
//        {
//            var buildMonitor = Substitute.For<IBuildMonitor>();
//            var buildMonitorFactory = Substitute.For<IBuildMonitorFactory>();
//            buildMonitorFactory.Create(Arg.Any<IInputDevice>()).Returns(buildMonitor);
//            var buildMonitorFactories = Substitute.For<IIndex<string, IBuildMonitorFactory>>();
//            buildMonitorFactories["key"].Returns(buildMonitorFactory);

//            var service = new SourceMonitoringService(buildMonitorFactories);

//            service.Start();

//            buildMonitor.DidNotReceive().BeginMonitoring();
//        }

//        [Fact]
//        public void should_begin_monitoring_when_started_if_mapping_exists()
//        {
//            var buildMonitor = Substitute.For<IBuildMonitor>();
//            var buildMonitorFactory = Substitute.For<IBuildMonitorFactory>();
//            buildMonitorFactory.Create(Arg.Any<IInputDevice>()).Returns(buildMonitor);
//            var buildMonitorFactories = Substitute.For<IIndex<string, IBuildMonitorFactory>>();
//            buildMonitorFactories["key"].ReturnsForAnyArgs(buildMonitorFactory);

//            var outputDevice = new OutputDeviceBuilder().Build();

//            var inputDevice = new InputDeviceBuilder().Build();

//            var mapping = new Mapping { OutputDeviceId = outputDevice.Id };
//            mapping.InputGroups.Add(new InputGroup { InputDeviceId = inputDevice.Id, Inputs = { "Build1", "Build2" } });

//            var config = new ConfigBuilder()
//                .WithOutputDevice(outputDevice)
//                .WithInputDevice(inputDevice)
//                .WithMapping(mapping)
//                .Build();

//            var service = new SourceMonitoringService(config, buildMonitorFactories);

//            service.Start();

//            buildMonitor.Received().BeginMonitoring();
//        }
//    }
//}
