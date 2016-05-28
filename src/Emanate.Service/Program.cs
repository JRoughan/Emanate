using Autofac;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Service.Api;
using Serilog;
using Serilog.Sinks.RollingFile;
using Topshelf;
using Topshelf.Autofac;

namespace Emanate.Service
{
    static class Program
    {
        static void Main()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .WriteTo.RollingFile(Paths.ServiceLogFilePath)
                .CreateLogger();

            var host = HostFactory.New(c =>
            {
                c.UseAutofacContainer(CreateContainer());

                //Settings.Initialize(c);

                c.UseSerilog(Log.Logger);

                c.Service<EmanateService>(s =>
                {
                    s.ConstructUsingAutofacContainer();
                    s.WhenStarted(es => es.Start());
                    s.WhenPaused(es => es.Pause());
                    s.WhenContinued(es => es.Continue());
                    s.WhenStopped(es => es.Stop());
                });

                c.SetServiceName(Settings.ServiceName);
                c.SetDisplayName("Emanate Monitoring Service");
                c.RunAsLocalSystem();
                c.StartAutomaticallyDelayed();
            });

            host.Run();
        }

        private static IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();
            var loader = new ModuleLoader();
            loader.LoadServiceModules(builder);

            builder.RegisterType<ConfigurationCaretaker>();
            builder.RegisterType<EmanateService>();
            builder.RegisterType<GlobalConfig>().OnActivating(async e =>
            {
                var caretaker = e.Context.Resolve<ConfigurationCaretaker>();
                e.ReplaceInstance(await caretaker.Load());
            });

            var container = builder.Build();

            Bootstrapper.SetContainer(container);

            return container;
        }
    }
}
