using System;
using System.Diagnostics;
using Autofac;
using Emanate.Core;
using Emanate.Core.Configuration;
using Topshelf;
using Topshelf.Autofac;

namespace Emanate.Service
{
    static class Program
    {
        public static string ServiceName = "EmanateService";
        static void Main()
        {
            HostFactory.Run(c =>
            {
                // Pass it to Topshelf
                c.UseAutofacContainer(CreateContainer());

                c.Service<EmanateService>(s =>
                {
                    s.ConstructUsingAutofacContainer();
                    s.WhenStarted(es => es.Start());
                    s.WhenPaused(es => es.Pause());
                    s.WhenContinued(es => es.Continue());
                    s.WhenStopped(es => es.Stop());
                });

                c.SetServiceName(ServiceName);
                c.SetDisplayName("Emanate Monitoring Service");
                c.RunAsLocalSystem();
                c.StartAutomaticallyDelayed();
            });
        }

        private static IContainer CreateContainer()
        {
            var bootStrapBuilder = new ContainerBuilder();
            var loader = new ModuleLoader();
            loader.LoadServiceModules(bootStrapBuilder);

            bootStrapBuilder.RegisterType<ConfigurationCaretaker>();
            bootStrapBuilder.RegisterType<EmanateService>();

            var container = bootStrapBuilder.Build();

            var caretaker = container.Resolve<ConfigurationCaretaker>();
            var config = caretaker.Load();

            var builder = new ContainerBuilder();
            builder.RegisterInstance(config);
            foreach (var moduleConfiguration in config.OututConfigurations)
            {
                Trace.TraceInformation("Registering module configuration '{0}'", moduleConfiguration.Name);
                builder.RegisterInstance(moduleConfiguration);
            }

            foreach (var moduleConfiguration in config.InputConfigurations)
            {
                Trace.TraceInformation("Registering module configuration '{0}'", moduleConfiguration.Name);
                builder.RegisterInstance(moduleConfiguration);
            }

            builder.Update(container);

            return container;
        }
    }
}
