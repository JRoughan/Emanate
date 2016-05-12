using System.Threading.Tasks;
using Autofac;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Service.Api;
using Serilog;
using Topshelf;
using Topshelf.Autofac;

namespace Emanate.Service
{
    static class Program
    {
        public static string ServiceName = "EmanateService";

        static void Main()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .CreateLogger();

            HostFactory.Run(async c =>
            {
                var container = await CreateContainer();
                c.UseAutofacContainer(container);

                c.UseSerilog(Log.Logger);

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

        private static async Task<IContainer> CreateContainer()
        {
            var bootStrapBuilder = new ContainerBuilder();
            var loader = new ModuleLoader();
            loader.LoadServiceModules(bootStrapBuilder);

            bootStrapBuilder.RegisterType<ConfigurationCaretaker>();
            bootStrapBuilder.RegisterType<EmanateService>();

            var container = bootStrapBuilder.Build();

            var caretaker = container.Resolve<ConfigurationCaretaker>();
            var config = await caretaker.Load();

            var builder = new ContainerBuilder();
            builder.RegisterInstance(config);
            foreach (var moduleConfiguration in config.OututConfigurations)
            {
                Log.Information("Registering module configuration '{0}'", moduleConfiguration.Name);
                builder.RegisterInstance(moduleConfiguration);
            }

            foreach (var moduleConfiguration in config.InputConfigurations)
            {
                Log.Information("Registering module configuration '{0}'", moduleConfiguration.Name);
                builder.RegisterInstance(moduleConfiguration);
            }

            builder.Update(container);

            Bootstrapper.SetContainer(container);

            return container;
        }
    }
}
