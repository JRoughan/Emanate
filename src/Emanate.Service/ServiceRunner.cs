using System;
using System.IO;
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
            var configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Emanate.config");
            var service = CreateApp<EmanateService>(configFile);

            var servicesToRun = new ServiceBase[] { service };
            ServiceBase.Run(servicesToRun);
        }

        public void RunAsConsole()
        {
            var consoleApp = CreateApp<EmanateConsole>();

            consoleApp.Start();
        }

        private T CreateApp<T>(string configFile = null)
            where T : EmanateService
        {
            var container = CreateContainer<T>();

            var caretaker = container.Resolve<ConfigurationCaretaker>();
            var config = caretaker.Load(configFile);

            // HACK: hard coded to teamcity
            var inputs = config.OutputDevices.SelectMany(d => d.Inputs).Where(i => i.Source == "teamcity");

            var builder = new ContainerBuilder();
            foreach (var moduleConfiguration in config.ModuleConfigurations)
                builder.RegisterInstance(moduleConfiguration);

            builder.Update(container);

            var app = container.Resolve<T>();
            app.SetInputsToMonitor(inputs);

            return app;
        }
    }
}