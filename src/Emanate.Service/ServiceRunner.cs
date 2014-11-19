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
            InitialiseConsole();
            var consoleApp = CreateApp<EmanateConsole>();

            consoleApp.Start();
        }

        private static void InitialiseConsole()
        {
            var handle = NativeMethods.GetConsoleWindow();
            if (handle == IntPtr.Zero)
                NativeMethods.AllocConsole();
            else
                NativeMethods.ShowWindow(handle, NativeMethods.SW_SHOW);

            Trace.Listeners.Add(new ConsoleTraceListener());
        }

        private T CreateApp<T>()
            where T : EmanateService
        {
            var container = CreateContainer<T>();

            var caretaker = container.Resolve<ConfigurationCaretaker>();
            var config = caretaker.Load();

            // HACK: hard coded to teamcity
            var inputs = config.OutputDevices.SelectMany(d => d.Inputs).Where(i => i.Source == "teamcity");

            var builder = new ContainerBuilder();
            foreach (var moduleConfiguration in config.ModuleConfigurations)
                builder.RegisterInstance(moduleConfiguration);

            builder.Update(container);

            var app = container.Resolve<T>();
            app.Initialize(config);

            return app;
        }
    }
}