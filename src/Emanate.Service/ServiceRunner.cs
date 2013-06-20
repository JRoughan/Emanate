using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceProcess;
using Autofac;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Output;

namespace Emanate.Service
{
    class ServiceRunner
    {
        private IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();
            var loader = new ModuleLoader();
            loader.LoadModules(builder);

            builder.RegisterType<ConfigurationCaretaker>();
            builder.RegisterType<EmanateService>();
            builder.RegisterType<EmanateConsole>();

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
            where T : IEmanateApp
        {
            var container = CreateContainer();
            var app = container.Resolve<T>();

            var caretaker = container.Resolve<ConfigurationCaretaker>();
            var config = caretaker.Load(configFile);

            //config.OutputDevices.SelectMany(d => d.)
            //app.SetInputsToMonitor(inputs);

            return app;
        }
    }

    public interface IEmanateApp
    {
        void SetInputsToMonitor(IEnumerable<InputInfo> inputsToMonitor);
    }
}