using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using Autofac;
using Emanate.Core;
using Emanate.Core.Configuration;

namespace Emanate.Service
{
    class ServiceRunner
    {
        private IContainer CreateContainer<TApp>()
        {
            var builder = new ContainerBuilder();
            var loader = new ModuleLoader();
            loader.LoadServiceModules(builder);

            builder.RegisterType<ConfigurationCaretaker>();
            builder.RegisterType<TApp>();

            return builder.Build();
        }

        public void RunAsService()
        {
            var service = CreateApp<EmanateService>();

            var servicesToRun = new ServiceBase[] { service };
            ServiceBase.Run(servicesToRun);
        }

        public void RunAsConsole()
        {
            Diagnostics.InitialiseConsole();
            var consoleApp = CreateApp<EmanateConsole>();

            consoleApp.Start();
        }

        private T CreateApp<T>()
            where T : EmanateService
        {
            Trace.TraceInformation("=> ServiceRunner.CreateApp<{0}>", typeof(T).Name);
            var container = CreateContainer<T>();

            var caretaker = container.Resolve<ConfigurationCaretaker>();
            var config = caretaker.Load();

            var builder = new ContainerBuilder();
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

            var app = container.Resolve<T>();
            app.Initialize(config);

            return app;
        }
    }
}